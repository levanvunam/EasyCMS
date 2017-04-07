using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Ez.Framework.Configurations;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Html;
using Ez.Framework.Utilities.Reflection;
using Ez.Framework.Utilities.Web;
using Ez.Framework.Utilities.Web.Models;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Exceptions.Sites;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.PageLogs;
using EzCMS.Core.Models.Pages;
using EzCMS.Core.Models.Pages.Widgets;
using EzCMS.Core.Models.Pages.Widgets.Member;
using EzCMS.Core.Models.Pages.Widgets.PageSearch;
using EzCMS.Core.Models.Pages.Logs;
using EzCMS.Core.Models.UserGroups;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Services.Widgets;
using EzCMS.Core.Services.PageLogs;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Core.Services.SocialMediaTokens;
using EzCMS.Core.Services.SubscriptionLogs;
using EzCMS.Core.Services.UserGroups;
using EzCMS.Core.Services.WorkConverters;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.Pages;
using EzCMS.Entity.Repositories.PageTags;
using HtmlAgilityPack;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Pages
{
    public class PageService : ServiceHelper, IPageService
    {
        private readonly IRepository<ClientNavigation> _clientNavigationRepository;
        private readonly IWidgetService _widgetService;
        private readonly IRepository<PageLog> _pageLogRepository;
        private readonly IPageLogService _pageLogService;
        private readonly IRepository<PageRead> _pageReadRepository;
        private readonly IPageRepository _pageRepository;
        private readonly IRepository<PageSecurity> _pageSecurityRepository;
        private readonly IPageTagRepository _pageTagRepository;
        private readonly ISiteSettingService _siteSettingService;
        private readonly ISocialMediaTokenService _socialMediaTokenService;
        private readonly ISubscriptionLogService _subscriptionLogService;
        private readonly IUserGroupService _userGroupService;

        public PageService(IPageLogService pageLogService,
            IWidgetService widgetService,
            ISiteSettingService siteSettingService,
            IUserGroupService userGroupService,
            ISubscriptionLogService subscriptionLogService,
            ISocialMediaTokenService socialMediaTokenService,
            IPageRepository pageRepository,
            IRepository<PageLog> pageLogRepository,
            IPageTagRepository pageTagRepository,
            IRepository<PageSecurity> pageSecurityRepository,
            IRepository<PageRead> pageReadRepository,
            IRepository<ClientNavigation> clientNavigationRepository
            )
        {
            _widgetService = widgetService;
            _pageLogService = pageLogService;
            _siteSettingService = siteSettingService;
            _userGroupService = userGroupService;
            _pageRepository = pageRepository;
            _pageLogRepository = pageLogRepository;
            _pageTagRepository = pageTagRepository;
            _subscriptionLogService = subscriptionLogService;
            _socialMediaTokenService = socialMediaTokenService;
            _pageSecurityRepository = pageSecurityRepository;
            _pageReadRepository = pageReadRepository;
            _clientNavigationRepository = clientNavigationRepository;
        }

        #region Logs

        /// <summary>
        /// Get page log model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="total"> </param>
        /// <param name="index"></param>
        /// <returns></returns>
        public PageLogListingModel GetLogs(int id, int total = 0, int index = 1)
        {
            var pageSize = _siteSettingService.GetSetting<int>(SettingNames.LogsPageSize);
            var page = GetById(id);
            if (page != null)
            {
                var logs = page.PageLogs.OrderByDescending(l => l.Created)
                    .GroupBy(l => l.SessionId)
                    .Skip((index - 1)*pageSize)
                    .Take(pageSize)
                    .ToList()
                    .Select(l => new PageLogsModel
                    {
                        SessionId = l.First().SessionId,
                        Creator = new SimpleUserModel(l.First().CreatedBy),
                        From = l.Last().Created,
                        To = l.First().Created,
                        Total = l.Count(),
                        Logs = l.Select(i => new PageLogItem(i)).ToList()
                    }).ToList();
                total = total + logs.Sum(l => l.Logs.Count);
                var model = new PageLogListingModel
                {
                    Id = page.Id,
                    Title = page.Title,
                    Total = total,
                    Logs = logs,
                    LoadComplete = total == page.PageLogs.Count
                };
                return model;
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Check if current user has permission to create new page or edit page
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public bool CanCurrentUserCreateOrEditPage(int? pageId = null)
        {
            var canEdit = false;
            var page = GetById(pageId);
            if (WorkContext.CurrentUser != null)
            {
                // System Administrator can always view and edit page
                if (WorkContext.CurrentUser.IsSystemAdministrator)
                {
                    canEdit = true;
                }
                else
                {
                    var groupIds = WorkContext.CurrentUser.GroupIds;

                    //When creating page, user may just need to have permission to edit any page to gain the create permission
                    if (page == null)
                    {
                        if (_pageSecurityRepository.Fetch(
                            pageSecurity => groupIds.Contains(pageSecurity.GroupId) && pageSecurity.CanEdit).Any())
                        {
                            canEdit = true;
                        }
                    }
                    else
                    {
                        // Check if current user can view or edit page or not
                        if (page.PageSecurities.Any())
                        {
                            canEdit =
                                page.PageSecurities.Where(s => s.CanEdit)
                                    .Any(s => groupIds.Contains(s.GroupId));
                        }
                        else
                        {
                            //Get all parent pages to check cascade secure
                            var allParentPages = _pageRepository.GetParents(page, false, false).ToList();

                            if (allParentPages.Any())
                            {
                                foreach (var item in allParentPages)
                                {
                                    /*
                                     * Get each parent page and check if there're any setup for securities
                                     * If any, then get the first setup
                                     */
                                    if (item.PageSecurities.Any())
                                    {
                                        canEdit =
                                            item.PageSecurities.Where(s => s.CanEdit)
                                                .Any(s => groupIds.Contains(s.GroupId));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return canEdit;
        }

        /// <summary>
        /// Get web page information
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public WebPageInformationModel GetWebPageInformationModel(int pageId)
        {
            var page = GetById(pageId);

            if (page == null)
            {
                return null;
            }

            return page.FriendlyUrl.ToAbsoluteUrl().DownloadPage();
        }

        #region Base

        public IQueryable<Page> GetAll()
        {
            return _pageRepository.GetAll();
        }

        public IQueryable<Page> Fetch(Expression<Func<Page, bool>> expression)
        {
            return _pageRepository.Fetch(expression);
        }

        public Page FetchFirst(Expression<Func<Page, bool>> expression)
        {
            return _pageRepository.FetchFirst(expression);
        }

        public Page GetById(object id)
        {
            return _pageRepository.GetById(id);
        }

        internal ResponseModel Insert(Page page)
        {
            return _pageRepository.Insert(page);
        }

        internal ResponseModel Update(Page page)
        {
            return _pageRepository.Update(page);
        }

        internal ResponseModel HierarchyUpdate(Page page)
        {
            return _pageRepository.HierarchyUpdate(page);
        }

        internal ResponseModel HierarchyInsert(Page page)
        {
            return _pageRepository.HierarchyInsert(page);
        }

        internal ResponseModel Delete(Page page)
        {
            return _pageRepository.Delete(page);
        }

        internal ResponseModel Delete(object id)
        {
            return _pageRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _pageRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Get

        /// <summary>
        /// Get page by friendly url
        /// </summary>
        /// <param name="friendlyUrl"></param>
        /// <returns></returns>
        public Page GetPage(string friendlyUrl)
        {
            //Get Home Page
            if (string.IsNullOrEmpty(friendlyUrl))
            {
                return GetHomePage();
            }

            return GetAll().FirstOrDefault(m =>
                (!m.StartPublishingDate.HasValue || DateTime.UtcNow > m.StartPublishingDate)
                && (!m.EndPublishingDate.HasValue || DateTime.UtcNow < m.StartPublishingDate)
                && m.FriendlyUrl.Equals(friendlyUrl, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Get page by friendly url
        /// </summary>
        /// <returns></returns>
        public Page GetPage(int? id)
        {
            return
                GetAll()
                    .FirstOrDefault(m => (!m.StartPublishingDate.HasValue || DateTime.UtcNow > m.StartPublishingDate)
                                         && (!m.EndPublishingDate.HasValue || DateTime.UtcNow < m.StartPublishingDate)
                                         && m.Id == id);
        }

        /// <summary>
        /// Get home page
        /// </summary>
        /// <returns></returns>
        public Page GetHomePage()
        {
            return _pageRepository.FetchFirst(p => p.IsHomePage);
        }

        /// <summary>
        /// Get all editable groups of page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<GroupItem> GetEditableGroups(int? id)
        {
            var page = GetById(id);
            if (page == null)
            {
                return new List<GroupItem>();
            }

            if (page.PageSecurities.Any())
            {
                return page.PageSecurities.Where(s => s.CanEdit)
                    .Select(s => new GroupItem
                    {
                        Id = s.GroupId,
                        Name = s.UserGroup.Name
                    }).ToList();
            }

            // Get all parent pages to check cascade secure
            var allParentPages = _pageRepository.GetParents(page, false, false).ToList();

            if (allParentPages.Any())
            {
                if (allParentPages.Any(item => item.PageSecurities.Any()))
                {
                    return page.PageSecurities.Where(s => s.CanEdit)
                        .Select(s => new GroupItem
                        {
                            Id = s.GroupId,
                            Name = s.UserGroup.Name
                        }).ToList();
                }
            }

            return new List<GroupItem>();
        }

        /// <summary>
        /// Get all viewable groups of page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<GroupItem> GetViewableGroups(int? id)
        {
            var page = GetById(id);
            if (page == null)
            {
                return new List<GroupItem>();
            }

            if (page.PageSecurities.Any())
            {
                return page.PageSecurities.Where(s => s.CanView)
                    .Select(s => new GroupItem
                    {
                        Id = s.GroupId,
                        Name = s.UserGroup.Name
                    }).ToList();
            }

            // Get all parent pages to check cascade secure
            var allParentPages = _pageRepository.GetParents(page, false, false).ToList();

            if (allParentPages.Any())
            {
                if (allParentPages.Any(item => item.PageSecurities.Any()))
                {
                    return page.PageSecurities.Where(s => s.CanView)
                        .Select(s => new GroupItem
                        {
                            Id = s.GroupId,
                            Name = s.UserGroup.Name
                        }).ToList();
                }
            }

            return new List<GroupItem>();
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the pages
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchPages(JqSearchIn si, PageSearchModel model)
        {
            var data = SearchPages(model);

            var pages = MapsAndReorder(data);

            return si.Search(pages);
        }

        /// <summary>
        /// Export pages
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, PageSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchPages(model);

            var pages = Maps(data);

            var exportData = si.Export(pages, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Search child pages
        /// </summary>
        /// <param name="si"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchChildrenPages(JqSearchIn si, int parentId)
        {
            var data = SearchChildPages(parentId);

            var pages = MapsAndReorder(data);

            return si.Search(pages);
        }

        /// <summary>
        /// Export pages
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public HSSFWorkbook ExportsChildrenPages(JqSearchIn si, GridExportMode gridExportMode, int parentId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchChildPages(parentId);

            var pages = Maps(data);

            var exportData = si.Export(pages, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="pages"></param>
        /// <returns></returns>
        public IQueryable<PageModel> Maps(IQueryable<Page> pages)
        {
            return pages.Select(p => new PageModel
            {
                Id = p.Id,
                Title = p.Title,
                FriendlyUrl = p.FriendlyUrl,
                Status = p.Status,
                IsHomePage = p.IsHomePage,
                TotalReads = p.PageReads.Count(),
                ParentId = p.ParentId,
                ParentName = p.ParentId.HasValue ? p.Parent.Title : string.Empty,
                Hierarchy = p.Hierarchy,
                PageTemplateId = p.PageTemplateId,
                PageTemplateName = p.PageTemplateId.HasValue ? p.PageTemplate.Name : string.Empty,
                FileTemplateId = p.FileTemplateId,
                FileTemplateName = p.FileTemplateId.HasValue ? p.FileTemplate.Name : string.Empty,
                BodyTemplateId = p.BodyTemplateId,
                BodyTemplateName = p.BodyTemplateId.HasValue ? p.BodyTemplate.Name : string.Empty,
                RecordOrder = p.RecordOrder,
                Created = p.Created,
                CreatedBy = p.CreatedBy,
                LastUpdate = p.LastUpdate,
                LastUpdateBy = p.LastUpdateBy
            });
        }

        #region Private methods

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="pages"></param>
        /// <returns></returns>
        public IQueryable<PageModel> MapsAndReorder(IQueryable<Page> pages)
        {
            var data = Maps(pages).ToList();

            var dictionary = data.ToDictionary(i => i.Id, i => i.RecordOrder);
            foreach (var item in data)
            {
                var prefix = string.Empty;
                var hierarchy = item.Hierarchy;
                var count = hierarchy.Count(c => c.Equals(FrameworkConstants.IdSeparator));
                for (var i = 0; i < count - 1; i++)
                {
                    prefix += FrameworkConstants.HierarchyLevelPrefix;
                }
                item.Title = prefix + item.Title;

                var hierarchyIds = new List<int>();
                if (!string.IsNullOrEmpty(hierarchy))
                {
                    hierarchyIds =
                        hierarchy.Substring(1, hierarchy.Length - 2)
                            .Split(FrameworkConstants.IdSeparator)
                            .Select(int.Parse)
                            .ToList();
                }

                var order = string.Empty;
                foreach (var id in hierarchyIds)
                {
                    int value;
                    if (dictionary.TryGetValue(id, out value))
                    {
                        order += string.Format("{0}{1}", FrameworkConstants.IdSeparator,
                            value.ToString(FrameworkConstants.HierarchyNodeFormat));
                    }
                }
                item.Hierarchy = order;
            }

            return data.AsQueryable();
        }

        /// <summary>
        /// Search pages
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<Page> SearchPages(PageSearchModel model)
        {
            if (model.TagIds == null) model.TagIds = new List<int>();

            var data = Fetch(p => (string.IsNullOrEmpty(model.Keyword) ||
                                   (!string.IsNullOrEmpty(p.Title) && p.Title.Contains(model.Keyword))
                                   || (!string.IsNullOrEmpty(p.FriendlyUrl) && p.FriendlyUrl.Contains(model.Keyword))
                                   || (!string.IsNullOrEmpty(p.Abstract) && p.Abstract.Contains(model.Keyword))
                                   ||
                                   (!string.IsNullOrEmpty(p.AbstractWorking) &&
                                    p.AbstractWorking.Contains(model.Keyword))
                                   || (!string.IsNullOrEmpty(p.Description) && p.Description.Contains(model.Keyword))
                                   || (!string.IsNullOrEmpty(p.Content) && p.Content.Contains(model.Keyword))
                                   ||
                                   (!string.IsNullOrEmpty(p.ContentWorking) && p.ContentWorking.Contains(model.Keyword))
                                   || (!string.IsNullOrEmpty(p.Keywords) && p.Keywords.Contains(model.Keyword))
                                   || (!string.IsNullOrEmpty(p.SeoTitle) && p.SeoTitle.Contains(model.Keyword)))
                                  && (!model.PageTemplateId.HasValue || p.PageTemplateId == model.PageTemplateId)
                                  && (!model.FileTemplateId.HasValue || p.FileTemplateId == model.FileTemplateId)
                                  && (!model.BodyTemplateId.HasValue || p.BodyTemplateId == model.BodyTemplateId)
                                  &&
                                  (!model.TagIds.Any() ||
                                   p.PageTags.Any(pageTag => model.TagIds.Contains(pageTag.TagId)))
                                  && (!model.Status.HasValue || p.Status == model.Status));

            return data;
        }

        /// <summary>
        /// Search child pages
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private IQueryable<Page> SearchChildPages(int parentId)
        {
            return Fetch(page => page.ParentId == parentId);
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Create a page manage model and populate data from parent and body template if provided
        /// </summary>
        /// <param name="parentId">id of parent page</param>
        /// <param name="bodyTemplateId">the body template id</param>
        /// <returns></returns>
        public PageManageModel GetPageManageModelWithParent(int? parentId = null, int? bodyTemplateId = null)
        {
            var parentPage = GetById(parentId);
            if (parentPage == null)
            {
                return new PageManageModel();
            }
            var manageModel = new PageManageModel(parentPage.Id, bodyTemplateId);
            return manageModel;
        }

        /// <summary>
        /// Get page manage model by id
        /// </summary>
        /// <param name="id">the page id</param>
        /// <returns></returns>
        public PageManageModel GetPageManageModel(int? id = null)
        {
            var page = GetById(id);
            return page != null ? new PageManageModel(page) : new PageManageModel();
        }

        /// <summary>
        /// Get page manage model by id
        /// </summary>
        /// <param name="id">the page id</param>
        /// <returns></returns>
        public PageManageModel GetPageManageModelByLogId(int? id = null)
        {
            var log = _pageLogRepository.GetById(id);
            return log != null ? new PageManageModel(log) : new PageManageModel();
        }

        /// <summary>
        /// Save page manage model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="confirmedChangeUrl"></param>
        /// <returns></returns>
        public ResponseModel SavePageManageModel(PageManageModel model, bool confirmedChangeUrl = false)
        {
            Page relativePage;
            ResponseModel response;
            var page = GetById(model.Id);

            #region Edit Page

            if (page != null)
            {
                var pageLog = new PageLogManageModel(page);
                var oldFriendlyUrl = page.FriendlyUrl;
                page.Title = model.Title;
                page.SeoTitle = model.SeoTitle;
                page.Keywords = model.Keywords;

                page.IncludeInSiteNavigation = model.IncludeInSiteNavigation;
                page.DisableNavigationCascade = model.DisableNavigationCascade;
                page.PageTemplateId = model.PageTemplateId;
                page.FileTemplateId = model.FileTemplateId;
                page.BodyTemplateId = model.BodyTemplateId;
                page.SSL = model.SSL;

                #region Get Model Content

                if (model.IsWordContent)
                {
                    var wordConverter = HostContainer.GetInstance<IWordConverter>();
                    var memStream = new MemoryStream();
                    model.File.InputStream.CopyTo(memStream);
                    var data = memStream.ToArray();
                    model.Content = wordConverter.ConvertDocxToHtmlByService(page.Id, data);
                }

                #endregion

                page.Status = model.Status;
                page.Description = model.Description;

                //Set content & abstract base on status
                if (model.Status == PageEnums.PageStatus.Draft)
                {
                    page.ContentWorking = model.Content;
                    page.AbstractWorking = model.Abstract;
                }
                else
                {
                    page.Content = model.Content;
                    page.Abstract = model.Abstract;
                    page.ContentWorking = string.Empty;
                    page.AbstractWorking = string.Empty;
                }

                #region Tags

                _pageTagRepository.SavePageTags(page, model.Tags);

                #endregion

                #region Page Securities

                var currentPageSecurities = page.PageSecurities.ToList();

                if (model.PageSecurityModels == null) model.PageSecurityModels = new List<PageSecurityModel>();

                // Remove useless sercurity
                foreach (var pageSecurityModel in model.PageSecurityModels.Where(s => !s.CanEdit && !s.CanView))
                {
                    var removedSecurity = currentPageSecurities.FirstOrDefault(s => s.Id == pageSecurityModel.Id);
                    if (removedSecurity != null)
                    {
                        _pageSecurityRepository.Delete(removedSecurity);
                    }
                }

                // Update sercurities
                if (model.PageSecurityModels != null && model.PageSecurityModels.Any())
                {
                    foreach (var pageSecurityModel in model.PageSecurityModels.Where(s => s.CanEdit || s.CanView))
                    {
                        var updatedSecurity = currentPageSecurities.FirstOrDefault(s => s.Id == pageSecurityModel.Id);
                        if (updatedSecurity != null)
                        {
                            updatedSecurity.CanEdit = pageSecurityModel.CanEdit;
                            updatedSecurity.CanView = pageSecurityModel.CanView;
                            _pageSecurityRepository.Update(updatedSecurity);
                        }
                        else
                        {
                            var insertedSecurity = new PageSecurity
                            {
                                GroupId = pageSecurityModel.GroupId,
                                CanEdit = pageSecurityModel.CanEdit,
                                CanView = pageSecurityModel.CanView,
                                PageId = page.Id
                            };
                            _pageSecurityRepository.Insert(insertedSecurity);
                        }
                    }
                }

                #endregion

                page.StartPublishingDate = model.StartPublishingDate;
                page.EndPublishingDate = model.EndPublishingDate;

                //Parse friendly url
                page.FriendlyUrl = string.IsNullOrWhiteSpace(model.FriendlyUrl)
                    ? model.Title.ToUrlString()
                    : model.FriendlyUrl.ToUrlString();

                #region Record Order

                //Get page record order
                relativePage = GetById(model.RelativePageId);
                if (relativePage != null)
                {
                    /*
                     * If position is not changed, donot need to update order of relative pages
                     * If position is changed, check if position is before or after and update the record other of all relative pages
                     */
                    var relativePages = Fetch(
                        p =>
                            p.Id != page.Id && relativePage.ParentId.HasValue
                                ? p.ParentId == relativePage.ParentId
                                : p.ParentId == null)
                        .OrderBy(p => p.RecordOrder);
                    if (model.Position == (int) PageEnums.PagePosition.Before)
                    {
                        if (page.RecordOrder > relativePage.RecordOrder ||
                            relativePages.Any(
                                p => p.RecordOrder > page.RecordOrder && p.RecordOrder < relativePage.RecordOrder))
                        {
                            page.RecordOrder = relativePage.RecordOrder;
                            var query =
                                string.Format(
                                    "Update Pages set RecordOrder = Order + 1 Where {0} And Order >= {1}",
                                    relativePage.ParentId.HasValue
                                        ? string.Format(" ParentId = {0}", relativePage.ParentId)
                                        : "ParentId Is NULL", relativePage.RecordOrder);
                            _pageRepository.ExcuteSql(query);
                        }
                    }
                    else
                    {
                        if (page.RecordOrder < relativePage.RecordOrder ||
                            relativePages.Any(
                                p => p.RecordOrder < page.RecordOrder && p.RecordOrder > relativePage.RecordOrder))
                        {
                            page.RecordOrder = relativePage.RecordOrder + 1;
                            var query =
                                string.Format(
                                    "Update Pages set RecordOrder = Order + 1 Where {0} And Order > {1}",
                                    relativePage.ParentId.HasValue
                                        ? string.Format(" ParentId = {0}", relativePage.ParentId)
                                        : "ParentId Is NULL", relativePage.RecordOrder);
                            _pageRepository.ExcuteSql(query);
                        }
                    }

                    int? clientNavigationParentId = null;
                    if (relativePage.ParentId != null)
                    {
                        clientNavigationParentId =
                            _clientNavigationRepository.FetchFirst(
                                clientNavigation => clientNavigation.PageId == relativePage.ParentId)
                                .Id;
                    }

                    //Update client Navigations
                    var clientNavigationsUpdateQuery =
                        string.Format(
                            "Update ClientNavigations set RecordOrder = Order + {0} Where PageId Is NULL And {1} And Order >= {2}",
                            EzCMSContants.OrderMultipleTimes,
                            clientNavigationParentId.HasValue
                                ? string.Format("ParentId = {0}", clientNavigationParentId)
                                : "ParentId Is NULL",
                            page.RecordOrder*EzCMSContants.OrderMultipleTimes);

                    _clientNavigationRepository.ExcuteSql(clientNavigationsUpdateQuery);
                }

                #endregion

                page.ParentId = model.ParentId;
                response = HierarchyUpdate(page);
                if (response.Success)
                {
                    _pageLogService.SavePageLog(pageLog);
                    _subscriptionLogService.SaveSubscriptionLog(model.Log);

                    _socialMediaTokenService.Post(model.SocialMessages, page);

                    var referencePages = GetReferencePagesViaLink(oldFriendlyUrl);

                    // Replace urls in reference pages
                    if (confirmedChangeUrl && referencePages.Any())
                    {
                        foreach (var referencePage in referencePages)
                        {
                            UpdateContentOfReferencePage(referencePage, oldFriendlyUrl, model.FriendlyUrl);
                        }
                    }
                }

                return response.SetMessage(response.Success
                    ? T("Page_Message_UpdateSuccessfully")
                    : T("Page_Message_UpdateFailure"));
            }

            #endregion

            Mapper.CreateMap<PageManageModel, Page>();
            page = Mapper.Map<PageManageModel, Page>(model);

            page.IsHomePage = false;
            page.FriendlyUrl = string.IsNullOrWhiteSpace(model.FriendlyUrl)
                ? model.Title.ToUrlString()
                : model.FriendlyUrl.ToUrlString();

            // Set content & description base on status
            if (model.Status == PageEnums.PageStatus.Draft)
            {
                page.ContentWorking = model.Content;
                page.AbstractWorking = model.Abstract;
            }

            #region Order

            //Get page record order
            relativePage = GetById(model.RelativePageId);
            if (relativePage != null)
            {
                if (model.Position == (int) PageEnums.PagePosition.Before)
                {
                    page.RecordOrder = relativePage.RecordOrder;
                    var query =
                        string.Format(
                            "Update Pages set RecordOrder = Order + 1 Where {0} And Order >= {1}",
                            relativePage.ParentId.HasValue
                                ? string.Format(" ParentId = {0}", relativePage.ParentId)
                                : "ParentId Is NULL", page.RecordOrder);
                    _pageRepository.ExcuteSql(query);
                }
                else
                {
                    page.RecordOrder = relativePage.RecordOrder + 1;
                    var query =
                        string.Format(
                            "Update Pages set RecordOrder = Order + 1 Where {0} And Order >= {1}",
                            relativePage.ParentId.HasValue
                                ? string.Format("ParentId = {0}", relativePage.ParentId)
                                : "ParentId Is NULL", page.RecordOrder);
                    _pageRepository.ExcuteSql(query);
                }

                int? clientNavigationParentId = null;
                if (relativePage.ParentId != null)
                {
                    clientNavigationParentId =
                        _clientNavigationRepository.FetchFirst(
                            clientNavigation => clientNavigation.PageId == relativePage.ParentId)
                            .Id;
                }

                //Update client Navigations
                var clientNavigationsUpdateQuery =
                    string.Format(
                        "Update ClientNavigations set RecordOrder = Order + {0} Where PageId Is Not NULL And {1} And Order >= {2}",
                        EzCMSContants.OrderMultipleTimes,
                        clientNavigationParentId.HasValue
                            ? string.Format("ParentId = {0}", clientNavigationParentId)
                            : "ParentId Is NULL",
                        page.RecordOrder*EzCMSContants.OrderMultipleTimes);

                _clientNavigationRepository.ExcuteSql(clientNavigationsUpdateQuery);
            }

            #endregion

            response = HierarchyInsert(page);

            if (response.Success)
            {
                _pageTagRepository.SavePageTags(page, model.Tags);

                #region Page Securities

                _pageSecurityRepository.Insert(
                    model.PageSecurityModels.Where(s => s.CanEdit || s.CanView).Select(p => new PageSecurity
                    {
                        GroupId = p.GroupId,
                        CanEdit = p.CanEdit,
                        CanView = p.CanView,
                        PageId = page.Id
                    }));

                #endregion

                // Convert from word content
                if (model.IsWordContent)
                {
                    var wordConverter = HostContainer.GetInstance<IWordConverter>();
                    var memStream = new MemoryStream();
                    model.File.InputStream.CopyTo(memStream);
                    var data = memStream.ToArray();
                    model.Content = wordConverter.ConvertDocxToHtmlByService(page.Id, data);
                    if (model.Status == PageEnums.PageStatus.Draft)
                    {
                        page.ContentWorking = model.Content;
                    }

                    Update(page);
                }
            }

            return
                response.SetMessage(response.Success
                    ? T("Page_Message_CreateSuccessfully")
                    : T("Page_Message_CreateFailure"));
        }

        #region Methods

        /// <summary>
        /// Save page order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SavePageOrder(PageOrderSetupModel model)
        {
            var manageModel = GetPageManageModel(model.CurrentId);

            if (manageModel.Id.HasValue)
            {
                // Save current order and parent
                var parentId = manageModel.ParentId;
                var order = manageModel.RecordOrder;

                manageModel.ParentId = model.ParentId;

                if (model.PreviousId.HasValue)
                {
                    manageModel.Position = (int) PageEnums.PagePosition.After;
                    manageModel.RelativePageId = model.PreviousId.Value;
                }
                else if (model.NextId.HasValue)
                {
                    manageModel.Position = (int) PageEnums.PagePosition.Before;
                    manageModel.RelativePageId = model.NextId.Value;
                }
                else
                {
                    manageModel.Position = (int) PageEnums.PagePosition.Before;
                    manageModel.RelativePageId = null;
                }

                var response = SavePageManageModel(manageModel);

                // Check if there is no change in order
                if (response.Success)
                {
                    var page = GetById(model.CurrentId);
                    if (page.ParentId == parentId ||
                        !page.ParentId.HasValue && !parentId.HasValue && page.RecordOrder == order)
                    {
                        return new ResponseModel
                        {
                            Success = true,
                            Message = string.Empty
                        };
                    }
                }

                return response.SetMessage(response.Success
                    ? T("Page_Message_UpdatePagePositionSuccessfully")
                    : T("Page_Message_UpdatePagePositionFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("Page_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Get all security settings for current page
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public List<PageSecurityModel> GetPageSecurities(int? pageId = null)
        {
            var result = new List<PageSecurityModel>();
            var pageSecurities = _pageSecurityRepository.GetAll().Where(ps => ps.PageId == pageId);
            var groups = _userGroupService.GetAll();
            foreach (var userGroup in groups)
            {
                if (pageSecurities.Any(ps => ps.GroupId == userGroup.Id))
                {
                    var pageSecurity = pageSecurities.First(ps => ps.GroupId == userGroup.Id);
                    result.Add(new PageSecurityModel
                    {
                        Id = pageSecurity.Id,
                        PageId = pageSecurity.Id,
                        GroupId = pageSecurity.GroupId,
                        CanEdit = pageSecurity.CanEdit,
                        CanView = pageSecurity.CanView,
                        GroupName = userGroup.Name
                    });
                }
                else
                {
                    result.Add(new PageSecurityModel(userGroup));
                }
            }
            return result;
        }

        /// <summary>
        /// Change home page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel ChangeHomePage(int id)
        {
            var response = new ResponseModel();
            var page = GetById(id);
            var homePage = GetHomePage();
            if (page != null)
            {
                if (page.Id != homePage.Id)
                {
                    homePage.IsHomePage = false;
                    page.IsHomePage = true;
                    if (Update(homePage).Success)
                    {
                        response = Update(page);
                        response.Message =
                            TFormat(
                                response.Success
                                    ? "Page_Message_ChangeHomePageSuccessfully"
                                    : "Page_Message_ChangeHomePageFailure", page.Title);
                    }
                }
                else
                {
                    response.Success = true;
                    response.Message = TFormat("Page_Message_ChangeHomePageSuccessfully", page.Title);
                }
            }
            else
            {
                response.Success = false;
                response.Message = T("Page_Message_ObjectNotFound");
            }
            return response;
        }

        /// <summary>
        /// Get possible parent Navigation
        /// </summary>
        /// <param name="id">the current Navigation id</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetPossibleParents(int? id = null)
        {
            var pages = GetAll();
            int? parentId = null;
            var page = GetById(id);
            if (page != null)
            {
                parentId = page.ParentId;
                pages = _pageRepository.GetPossibleParents(page);
            }
            var data = pages.Select(m => new HierarchyDropdownModel
            {
                Id = m.Id,
                Name = m.Title,
                Hierarchy = m.Hierarchy,
                RecordOrder = m.RecordOrder,
                Selected = parentId.HasValue && parentId.Value == m.Id
            }).ToList();
            return _pageRepository.BuildSelectList(data);
        }

        /// <summary>
        /// Get status of page
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetStatus()
        {
            return EnumUtilities.GenerateSelectListItems<PageEnums.PageStatus>();
        }

        /// <summary>
        /// Get page by parent id
        /// </summary>
        /// <param name="position"> </param>
        /// <param name="relativePageId"> </param>
        /// <param name="pageId"> </param>
        /// <param name="parentId">the parent id</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetRelativePages(out int position, out int relativePageId, int? pageId = null,
            int? parentId = null)
        {
            position = (int) PageEnums.PagePosition.Before;
            relativePageId = 0;
            var order = 0;
            var relativePages =
                Fetch(
                    p =>
                        (!pageId.HasValue || p.Id != pageId) &&
                        (parentId.HasValue ? p.ParentId == parentId : p.ParentId == null))
                    .OrderBy(p => p.RecordOrder).Select(p => new
                    {
                        p.Title,
                        p.Id,
                        p.RecordOrder
                    }).ToList();
            var page = GetById(pageId);
            if (page != null)
            {
                order = page.RecordOrder;
            }

            //Flag to check if current page is the bigest order in relative page list
            var flag = false;
            for (var i = 0; i < relativePages.Count(); i++)
            {
                if (relativePages[i].RecordOrder > order)
                {
                    relativePageId = relativePages[i].Id;
                    flag = true;
                    break;
                }
            }
            if (!flag && relativePages.Any())
            {
                position = (int) PageEnums.PagePosition.After;
                relativePageId = relativePages.Last().Id;
            }
            var selectPageId = relativePageId;
            return relativePages.Select(p => new SelectListItem
            {
                Text = p.Title,
                Value = p.Id.ToString(CultureInfo.InvariantCulture),
                Selected = p.Id == selectPageId
            });
        }

        /// <summary>
        /// Get page by parent id
        /// </summary>
        /// <param name="pageId"> </param>
        /// <param name="parentId">the parent id</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetRelativePages(int? pageId = null, int? parentId = null)
        {
            return Fetch(p => (!pageId.HasValue || p.Id != pageId)
                              && (parentId.HasValue ? p.ParentId == parentId : p.ParentId == null))
                .OrderBy(p => p.RecordOrder).Select(p => new SelectListItem
                {
                    Text = p.Title,
                    Value = SqlFunctions.StringConvert((double) p.Id).Trim()
                });
        }

        /// <summary>
        /// Get visible pages of current user
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetAccessablePageSelectList()
        {
            return GetAccessablePages()
                .Select(p => new SelectListItem
                {
                    Text = p.Title,
                    Value = SqlFunctions.StringConvert((double) p.Id).Trim()
                });
        }

        /// <summary>
        /// Get visible pages of current user
        /// </summary>
        /// <returns></returns>
        public IQueryable<Page> GetAccessablePages()
        {
            var currentUserGroups = new List<int>();
            if (WorkContext.CurrentUser != null)
            {
                currentUserGroups = WorkContext.CurrentUser.GroupIds;
            }
            return
                Fetch(
                    p =>
                        !p.PageSecurities.Any() ||
                        p.PageSecurities.Any(ps => ps.CanView && currentUserGroups.Contains(ps.GroupId)));
        }

        #endregion

        #region SEO Scoring

        /// <summary>
        /// Get SEO score
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="content"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public SEOScoringModel GetSEOScoringModel(string title, string description, string content, string keywords)
        {
            var seoScoringSetting = _siteSettingService.LoadSetting<SEOScoringSetting>();

            PageEnums.SEOScore keywordCountScore, keywordWeightScore, keywordBoldedScore, headingTagScore, altTagScore;
            GetKeywordsChangedSEOScore(content, keywords, out keywordCountScore, out keywordWeightScore,
                out keywordBoldedScore, out headingTagScore, out altTagScore);

            var model = new SEOScoringModel
            {
                Title = GetTitleChangedSEOScore(title, seoScoringSetting),
                Description = GetTitleChangedSEOScore(description, seoScoringSetting),
                KeywordCount = keywordCountScore,
                KeywordWeight = keywordWeightScore,
                KeywordBolded = keywordBoldedScore,
                HeadingTag = headingTagScore,
                AltTag = altTagScore
            };

            return model;
        }

        /// <summary>
        /// Get SEO score when changing title
        /// </summary>
        /// <param name="title"></param>
        /// <param name="seoScoringSetting"></param>
        /// <returns></returns>
        public PageEnums.SEOScore GetTitleChangedSEOScore(string title, SEOScoringSetting seoScoringSetting = null)
        {
            if (seoScoringSetting == null)
            {
                seoScoringSetting = _siteSettingService.LoadSetting<SEOScoringSetting>();
            }

            PageEnums.SEOScore score;

            if (string.IsNullOrEmpty(title) || title.Length < seoScoringSetting.TitleGoodRangeFrom)
            {
                score = PageEnums.SEOScore.Bad;
            }
            else if (title.Length >= seoScoringSetting.TitleGoodRangeFrom &&
                     title.Length <= seoScoringSetting.TitleGoodRangeTo)
            {
                score = PageEnums.SEOScore.Good;
            }
            else
            {
                score = PageEnums.SEOScore.Medium;
            }

            return score;
        }

        /// <summary>
        /// Get SEO score when changing description
        /// </summary>
        /// <param name="description"></param>
        /// <param name="seoScoringSetting"></param>
        /// <returns></returns>
        public PageEnums.SEOScore GetDescriptionChangedSEOScore(string description,
            SEOScoringSetting seoScoringSetting = null)
        {
            if (seoScoringSetting == null)
            {
                seoScoringSetting = _siteSettingService.LoadSetting<SEOScoringSetting>();
            }
            PageEnums.SEOScore score;

            if (string.IsNullOrEmpty(description) || description.Length < seoScoringSetting.DescriptionGoodRangeFrom)
            {
                score = PageEnums.SEOScore.Bad;
            }
            else if (description.Length >= seoScoringSetting.DescriptionGoodRangeFrom &&
                     description.Length <= seoScoringSetting.DescriptionGoodRangeTo)
            {
                score = PageEnums.SEOScore.Good;
            }
            else
            {
                score = PageEnums.SEOScore.Medium;
            }

            return score;
        }

        /// <summary>
        /// Get SEO score when changing content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="keywords"></param>
        /// <param name="keywordWeightScore"></param>
        /// <param name="keywordBoldedScore"></param>
        /// <param name="headingScore"></param>
        /// <param name="altScore"></param>
        /// <param name="seoScoringSetting"></param>
        public void GetContentChangedSEOScore(string content, string keywords, out PageEnums.SEOScore keywordWeightScore,
            out PageEnums.SEOScore keywordBoldedScore, out PageEnums.SEOScore headingScore,
            out PageEnums.SEOScore altScore, SEOScoringSetting seoScoringSetting = null)
        {
            // If no keywords then get default one
            if (string.IsNullOrEmpty(keywords))
            {
                var companySetupSetting = _siteSettingService.LoadSetting<CompanySetupSetting>();
                keywords = companySetupSetting.Keywords;
            }

            if (seoScoringSetting == null)
            {
                seoScoringSetting = _siteSettingService.LoadSetting<SEOScoringSetting>();
            }

            // Empty content return nothing
            if (string.IsNullOrEmpty(content))
            {
                keywordWeightScore = PageEnums.SEOScore.Bad;
                keywordBoldedScore = PageEnums.SEOScore.Bad;
                headingScore = PageEnums.SEOScore.VeryBad;
                altScore = PageEnums.SEOScore.Good;
            }
            else
            {
                var document = new HtmlDocument();
                document.LoadHtml(content);

                var keywordList = string.IsNullOrEmpty(keywords)
                    ? new List<string>()
                    : keywords.Split(new[] {FrameworkConstants.Colon}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(keyword => keyword.Trim())
                        .Where(keyword => !string.IsNullOrEmpty(keyword))
                        .ToList();

                #region Keyword Weight

                var plainTextFromContent = content.ConvertHtmlToText();
                double words = plainTextFromContent.Split(null).Count();
                double matchs = 0;
                foreach (var keyword in keywordList)
                {
                    matchs += Regex.Matches(plainTextFromContent, keyword).Count;
                }

                var keywordWeight = matchs/words*100;

                if (keywordWeight < seoScoringSetting.KeywordWeightGoodRangeFrom)
                {
                    keywordWeightScore = PageEnums.SEOScore.Bad;
                }
                else if (keywordWeight >= seoScoringSetting.KeywordWeightGoodRangeFrom &&
                         keywordWeight <= seoScoringSetting.KeywordWeightGoodRangeTo)
                {
                    keywordWeightScore = PageEnums.SEOScore.Good;
                }
                else
                {
                    keywordWeightScore = PageEnums.SEOScore.Medium;
                }

                #endregion

                #region Keyword Bolded

                var boldedTags = document.DocumentNode.Descendants()
                    .Where(node => node.Name == "b" || node.Name == "strong")
                    .ToList();

                if (boldedTags.Any())
                {
                    var keywordAllBolded = true;
                    var noKeywordBolded = true;
                    foreach (var keyword in keywordList)
                    {
                        if (boldedTags.Any(t => t.InnerText.Contains(keyword)))
                        {
                            noKeywordBolded = false;
                        }
                        else
                        {
                            keywordAllBolded = false;
                        }

                        // Break if there is some keywords in bold tags
                        if (!noKeywordBolded && !keywordAllBolded)
                        {
                            break;
                        }
                    }

                    if (keywordAllBolded)
                    {
                        keywordBoldedScore = PageEnums.SEOScore.Good;
                    }
                    else if (noKeywordBolded)
                    {
                        keywordBoldedScore = PageEnums.SEOScore.Bad;
                    }
                    else
                    {
                        keywordBoldedScore = PageEnums.SEOScore.Medium;
                    }
                }
                else
                {
                    keywordBoldedScore = PageEnums.SEOScore.Bad;
                }

                #endregion

                #region Heading Tags

                var headingTags = document.DocumentNode.Descendants().Where(node => node.Name == "h1").ToList();
                if (headingTags.Any())
                {
                    headingScore = PageEnums.SEOScore.Bad;
                    foreach (var headingTag in headingTags)
                    {
                        var headingWords = headingTag.InnerText.Split(' ');
                        if (keywordList.Any(headingWords.Contains))
                        {
                            headingScore = PageEnums.SEOScore.Good;
                            break;
                        }
                    }
                }
                else
                {
                    headingScore = PageEnums.SEOScore.VeryBad;
                }

                #endregion

                #region Alt Tags

                var imageTags = document.DocumentNode.Descendants().Where(node => node.Name == "img").ToList();

                if (imageTags.Any())
                {
                    var keywordInAllAltTags = true;
                    var noKeywordInAllAltTags = true;
                    foreach (var image in imageTags)
                    {
                        var alt = image.Attributes["alt"];
                        if (alt == null || string.IsNullOrEmpty(alt.Value) ||
                            !keywordList.Any(keyword => alt.Value.Contains(keyword)))
                        {
                            keywordInAllAltTags = false;
                        }
                        else if (keywordList.Any(keyword => alt.Value.Contains(keyword)))
                        {
                            noKeywordInAllAltTags = false;
                        }

                        //Break if there are keyword in some of tags
                        if (!keywordInAllAltTags && !noKeywordInAllAltTags) break;
                    }

                    if (keywordInAllAltTags)
                    {
                        altScore = PageEnums.SEOScore.Good;
                    }
                    else if (noKeywordInAllAltTags)
                    {
                        altScore = PageEnums.SEOScore.Bad;
                    }
                    else
                    {
                        altScore = PageEnums.SEOScore.Medium;
                    }
                }
                else
                {
                    altScore = PageEnums.SEOScore.Good;
                }

                #endregion
            }
        }

        /// <summary>
        /// Get SEO score when changing keywords
        /// </summary>
        /// <param name="content"></param>
        /// <param name="keywords"></param>
        /// <param name="keywordCountScore"></param>
        /// <param name="keywordWeightScore"></param>
        /// <param name="keywordBoldedScore"></param>
        /// <param name="headingScore"></param>
        /// <param name="altScore"></param>
        /// <param name="seoScoringSetting"></param>
        /// <returns></returns>
        public void GetKeywordsChangedSEOScore(string content, string keywords, out PageEnums.SEOScore keywordCountScore,
            out PageEnums.SEOScore keywordWeightScore, out PageEnums.SEOScore keywordBoldedScore,
            out PageEnums.SEOScore headingScore, out PageEnums.SEOScore altScore,
            SEOScoringSetting seoScoringSetting = null)
        {
            // If no keywords then get default one
            if (string.IsNullOrEmpty(keywords))
            {
                var companySetupSetting = _siteSettingService.LoadSetting<CompanySetupSetting>();
                keywords = companySetupSetting.Keywords;
            }

            if (seoScoringSetting == null)
            {
                seoScoringSetting = _siteSettingService.LoadSetting<SEOScoringSetting>();
            }

            #region Keywords

            var keywordList = string.IsNullOrEmpty(keywords)
                ? new List<string>()
                : keywords.Split(new[] {FrameworkConstants.Colon}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(keyword => keyword.Trim())
                    .Where(keyword => !string.IsNullOrEmpty(keyword))
                    .ToList();

            var keywordCount = keywordList.Count();

            if (keywordCount < seoScoringSetting.KeywordCountGoodRangeFrom)
            {
                keywordCountScore = PageEnums.SEOScore.Bad;
            }
            else if (keywordCount >= seoScoringSetting.KeywordCountGoodRangeFrom &&
                     keywordCount <= seoScoringSetting.KeywordCountGoodRangeTo)
            {
                keywordCountScore = PageEnums.SEOScore.Good;
            }
            else
            {
                keywordCountScore = PageEnums.SEOScore.Medium;
            }

            #endregion

            GetContentChangedSEOScore(content, keywords, out keywordWeightScore, out keywordBoldedScore,
                out headingScore, out altScore, seoScoringSetting);
        }

        #endregion

        #endregion

        #region Delete Page

        /// <summary>
        /// Get page delete model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PageDeleteModel GetPageDeleteModel(int id)
        {
            var page = GetById(id);
            if (page != null)
            {
                return new PageDeleteModel(page);
            }

            return null;
        }

        /// <summary>
        /// Get reference pages
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public IQueryable<Page> GetReferencePages(Page page)
        {
            var textWidget = string.Format("{{PageLink_{0}}}", page.Id);
            var linkWidget = string.Format("{{PageLink_{0}_", page.Id);

            var pagesIdViaLink = GetReferencePagesViaLink(page.FriendlyUrl).Select(p => p.Id);
            return Fetch(p => p.Id != page.Id
                              && (pagesIdViaLink.Contains(p.Id)
                                  || p.Content.Contains(textWidget)
                                  || p.Content.Contains(linkWidget)));
        }

        /// <summary>
        /// Replace page url
        /// </summary>
        /// <param name="fromPage"></param>
        /// <param name="content"></param>
        /// <param name="newUrl"></param>
        /// <param name="newPageId"></param>
        /// <returns></returns>
        private string ReplacePageUrl(Page fromPage, string content, string newUrl, int? newPageId)
        {
            /*
             * This function will replace 2 kind of url in page content
             *  * Friendly url in anchor href attribute
             *  * {PageLink} widget
             */

            #region Replace Url

            var possibleFriendlyUrls = new List<string>
            {
                fromPage.FriendlyUrl,
                fromPage.FriendlyUrl.ToPageFriendlyUrl(),
                fromPage.FriendlyUrl.ToAbsoluteUrl()
            };

            foreach (var possibleFriendlyUrl in possibleFriendlyUrls)
            {
                content = content.ReplaceUrl(possibleFriendlyUrl, newUrl);
            }

            #endregion

            #region Replace Widget

            if (newPageId.HasValue)
            {
                //Replace text widget
                var textWidget = string.Format("{{PageLink_{0}}}", fromPage.Id);
                var newTextWidget = string.Format("{{PageLink_{0}}}", newPageId);
                content = content.Replace(textWidget, newTextWidget);

                // Replace link widget
                var linkWidget = string.Format("{{PageLink_{0}_", fromPage.Id);
                var newLinkWidget = string.Format("{{PageLink_{0}_", newPageId);

                content = content.Replace(linkWidget, newLinkWidget);
            }

            #endregion

            return content;
        }

        /// <summary>
        /// Delete page and its related data by page id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel DeletePage(PageDeleteModel model)
        {
            var page = GetById(model.Id);

            if (page != null)
            {
                if (page.IsHomePage)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Page_Message_CanNotDeleteHomePage")
                    };
                }

                if (page.ChildrenPages.Any())
                    if (page.ChildrenPages.Any())
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = T("Page_Message_DeleteFailureBasedOnRelatedChildren")
                        };
                    }

                if (page.LinkTrackers.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Page_Message_DeleteFailureBasedOnRelatedLinkTrackers")
                    };
                }

                _pageTagRepository.Delete(page.PageTags);

                _pageSecurityRepository.Delete(page.PageSecurities);

                _pageLogRepository.Delete(page.PageLogs);

                #region Replace Url

                //Get all reference pages that link to current page
                var referencePages = GetReferencePages(page).ToList();

                var newUrl = string.Empty;
                int? newPageId = null;
                if (model.UrlType == CommonEnums.UrlType.Internal)
                {
                    var newPage = GetById(model.ReplacePageId);
                    if (newPage != null)
                    {
                        newUrl = newPage.FriendlyUrl.ToPageFriendlyUrl();
                        newPageId = model.ReplacePageId;
                    }
                }
                else
                {
                    newUrl = model.ReplaceUrl;
                }

                foreach (var referencePage in referencePages)
                {
                    referencePage.Content = ReplacePageUrl(page, referencePage.Content, newUrl, newPageId);
                    Update(referencePage);
                }

                #endregion

                var response = Delete(page);

                return response.SetMessage(response.Success
                    ? T("Page_Message_DeleteSuccessfully")
                    : T("Page_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("Page_Message_DeleteSuccessfully")
            };
        }

        /// <summary>
        /// Remove relationship between page and body template
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="bodyTemplateId"></param>
        /// <returns></returns>
        public ResponseModel DeletePageBodyTemplateMapping(int pageId, int bodyTemplateId)
        {
            var page = _pageRepository.FetchFirst(p => p.Id == pageId && p.BodyTemplateId == bodyTemplateId);

            if (page != null)
            {
                page.BodyTemplateId = null;
                var response = Update(page);
                return response.SetMessage(response.Success
                    ? T("PageBodyTemplate_Message_DeleteMappingSuccessfully")
                    : T("PageBodyTemplate_Message_DeleteMappingFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("PageBodyTemplate_Message_DeleteMappingSuccessfully")
            };
        }

        /// <summary>
        /// Remove relationship between page and page template
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="pageTemplateId"></param>
        /// <returns></returns>
        public ResponseModel DeletePagePageTemplateMapping(int pageId, int pageTemplateId)
        {
            var page = _pageRepository.FetchFirst(p => p.Id == pageId && p.PageTemplateId == pageTemplateId);

            if (page != null)
            {
                page.PageTemplateId = null;
                var response = Update(page);
                return response.SetMessage(response.Success
                    ? T("PagePageTemplate_Message_DeleteMappingSuccessfully")
                    : T("PagePageTemplate_Message_DeleteMappingFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("PagePageTemplate_Message_DeleteMappingSuccessfully")
            };
        }

        /// <summary>
        /// Remove relationship between page and file template
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="fileTemplateId"></param>
        /// <returns></returns>
        public ResponseModel DeletePageFileTemplateMapping(int pageId, int fileTemplateId)
        {
            var page = _pageRepository.FetchFirst(p => p.Id == pageId && p.FileTemplateId == fileTemplateId);

            if (page != null)
            {
                page.FileTemplateId = null;
                var response = Update(page);
                return response.SetMessage(response.Success
                    ? T("PageFileTemplate_Message_DeleteMappingSuccessfully")
                    : T("PageFileTemplate_Message_DeleteMappingFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("PageFileTemplate_Message_DeleteMappingSuccessfully")
            };
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check if status is valid or not
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool IsHomepageOffline(int? pageId, PageEnums.PageStatus status)
        {
            if (status == PageEnums.PageStatus.Offline)
            {
                return Fetch(u => u.IsHomePage && u.Id == pageId).Any();
            }
            return false;
        }

        /// <summary>
        /// Check if friendly url exists
        /// </summary>
        /// <param name="pageId">the page id</param>
        /// <param name="friendlyUrl">the friendly url</param>
        /// <returns></returns>
        public bool IsFriendlyUrlExisted(int? pageId, string friendlyUrl)
        {
            return Fetch(u => u.FriendlyUrl.Equals(friendlyUrl) && u.Id != pageId).Any();
        }

        #endregion

        #region Render

        /// <summary>
        /// Render page content by page id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="showDraft"></param>
        /// <returns></returns>
        public PageRenderModel RenderContent(int? id, bool showDraft = false)
        {
            var page = GetPage(id);
            return RenderContent(page, showDraft);
        }

        /// <summary>
        /// Render page content by friendly url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="showDraft"></param>
        /// <returns></returns>
        public PageRenderModel RenderContent(string url, bool showDraft = false)
        {
            var page = GetPage(url);
            if (page == null)
            {
                // Check url is end with '/'
                if (url.EndsWith("/"))
                {
                    var newUrl = url.Substring(0, url.Length - 1);
                    var newPage = GetPage(newUrl);
                    if (newPage != null)
                    {
                        //Reurn 301 redirect
                        return new PageRenderModel
                        {
                            ResponseCode = PageEnums.PageResponseCode.Redirect301,
                            Redirect301Url = newUrl
                        };
                    }
                }
            }

            return RenderContent(page, showDraft);
        }

        /// <summary>
        /// Render page content
        /// </summary>
        /// <param name="page"></param>
        /// <param name="showDraft"></param>
        /// <returns></returns>
        public PageRenderModel RenderContent(Page page, bool showDraft = false)
        {
            if (page != null)
            {
                if (page.SSL && HttpContext.Current.Request.Url.Scheme.ToLower() == "http")
                {
                    return new PageRenderModel
                    {
                        ResponseCode = PageEnums.PageResponseCode.RequiredSSL
                    };
                }

                WorkContext.ActivePageId = page.Id;

                var model = new PageRenderModel(page, showDraft);

                #region Security

                // System Administrator can always view and edit page
                if (WorkContext.CurrentUser != null && WorkContext.CurrentUser.IsSystemAdministrator)
                {
                    model.IsLoggedIn = true;
                    model.CanEdit = true;
                    model.CanView = true;
                    model.GroupIds = _userGroupService.GetAll().Select(g => g.Id).ToList();
                }
                else
                {
                    if (WorkContext.CurrentUser != null)
                    {
                        model.IsLoggedIn = true;
                        model.GroupIds = WorkContext.CurrentUser.GroupIds;
                    }
                    else
                    {
                        model.IsLoggedIn = false;
                        model.GroupIds = new List<int>();
                    }

                    // Check if current user can view or edit page or not
                    if (page.PageSecurities.Any())
                    {
                        model.CanView =
                            page.PageSecurities.Where(s => s.CanView)
                                .Any(s => model.GroupIds.Contains(s.GroupId));

                        model.CanEdit =
                            page.PageSecurities.Where(s => s.CanEdit)
                                .Any(s => model.GroupIds.Contains(s.GroupId));
                    }
                    else
                    {
                        //Set default value
                        model.CanView = true;
                        model.CanEdit = false;

                        //Get all parent pages to check cascade secure
                        var allParentPages = _pageRepository.GetParents(page, false, false).ToList();

                        if (allParentPages.Any())
                        {
                            foreach (var item in allParentPages)
                            {
                                /*
                                 * Get each parent page and check if there're any setup for securities
                                 * If any, then get the first setup
                                 */
                                if (item.PageSecurities.Any())
                                {
                                    model.CanView =
                                        item.PageSecurities.Where(s => s.CanView)
                                            .Any(s => model.GroupIds.Contains(s.GroupId));

                                    model.CanEdit =
                                        item.PageSecurities.Where(s => s.CanEdit)
                                            .Any(s => model.GroupIds.Contains(s.GroupId));
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!model.CanView)
                {
                    throw new EzCMSUnauthorizeException();
                }

                #endregion

                // Offline page only visible to editable users
                if (model.Status == PageEnums.PageStatus.Offline)
                {
                    if (model.CanEdit)
                    {
                        model.ResponseCode = PageEnums.PageResponseCode.PageOffline;
                    }
                    else
                    {
                        throw new EzCMSNotFoundException(HttpContext.Current.Request.RawUrl);
                    }
                }

                // Page draft
                if (model.Status == PageEnums.PageStatus.Draft && model.CanEdit &&
                    !string.Equals(page.Content, page.ContentWorking))
                {
                    if (model.ShowDraft)
                    {
                        model.Content = page.ContentWorking;
                    }
                    else
                    {
                        model.ResponseCode = PageEnums.PageResponseCode.PageDraft;
                    }
                }

                // Check if the page template is valid or not
                if (page.PageTemplate != null && !page.PageTemplate.IsValid)
                {
                    model.Content = page.PageTemplate.CompileMessage;
                }
                else
                {
                    var templateCacheName = page.PageTemplate != null ? page.PageTemplate.CacheName : null;

                    var pageTemplateHtml = EzCMSContants.RenderPageContent;
                    if (!string.IsNullOrEmpty(templateCacheName))
                    {
                        pageTemplateHtml = pageTemplateHtml.InsertMasterPage(templateCacheName);
                    }

                    // Get template cache name from page title and Template 
                    var pageTemplateCacheName = page.Title.GetTemplateCacheName(pageTemplateHtml);

                    var pageContent = EzRazorEngineHelper.CompileAndRun(pageTemplateHtml, model, null,
                        pageTemplateCacheName);

                    model.Content = _widgetService.Render(pageContent);
                }

                return model;
            }

            throw new EzCMSNotFoundException(HttpContext.Current.Request.RawUrl);
        }

        /// <summary>
        /// Setup page tracking
        /// </summary>
        /// <param name="pageId"></param>
        public ResponseModel SetupPageTracking(int pageId)
        {
            #region Setup page read

            var anonymousContactId = WorkContext.CurrentContact.AnonymousContactId;
            if (anonymousContactId > 0)
            {
                var pageRead = new PageRead
                {
                    PageId = pageId,
                    AnonymousContactId = anonymousContactId
                };

                return _pageReadRepository.Insert(pageRead);
            }

            return new ResponseModel
            {
                Success = true
            };

            #endregion
        }

        #endregion

        #region Widgets

        #region Members & Breadcrumbs

        /// <summary>
        /// Get breadcrums of current page
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public BreadcrumbsWidget GetBreadcrumbs(int? pageId)
        {
            var page = GetById(pageId);

            if (page == null)
            {
                return null;
            }

            var parentPages = _pageRepository.GetParents(page, false);

            return new BreadcrumbsWidget
            {
                CurrentBreadcrumb = new MemberItem(page),
                Breadcrumbs =
                    parentPages == null
                        ? new List<MemberItem>()
                        : parentPages.ToList().Select(p => new MemberItem(p)).ToList()
            };
        }

        /// <summary>
        /// Get breadcrums of current page
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public MembersWidget GetMembers(int? pageId)
        {
            var page = GetById(pageId);

            if (page == null)
            {
                return null;
            }

            return new MembersWidget(page);
        }

        #endregion

        #region Search Pages

        /// <summary>
        /// Search page by keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public PageSearchWidget SearchPages(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return new PageSearchWidget();
            }

            return new PageSearchWidget(keyword);
        }

        #endregion

        #region Page Content

        /// <summary>
        /// Get page render model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PageRenderModel GetPageRenderModel(int id)
        {
            var page = GetById(id);

            if (page != null)
            {
                return new PageRenderModel(page);
            }

            return null;
        }

        #endregion

        #region Page Link

        /// <summary>
        /// Get page link
        /// </summary>
        /// <param name="id"></param>
        /// <param name="renderHtmlLink"></param>
        /// <param name="className"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public PageLinkWidget GetPageLinkWidget(int id, bool renderHtmlLink, string className, string title)
        {
            var pageLink = GetById(id);

            if (pageLink != null) return new PageLinkWidget(pageLink, renderHtmlLink, className, title);

            return null;
        }

        #endregion

        #endregion

        #region Update Reference Url

        /// <summary>
        /// Get page confirm change url model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newFriendlyUrl"></param>
        /// <returns></returns>
        public PageConfirmChangeUrlModel GetPageConfirmChangeUrlModel(int id, string newFriendlyUrl)
        {
            var page = GetById(id);
            if (page != null && !page.FriendlyUrl.Equals(newFriendlyUrl))
            {
                var referencePages = GetReferencePagesViaLink(page.FriendlyUrl);
                if (referencePages.Any())
                {
                    var model = new PageConfirmChangeUrlModel
                    {
                        NewUrl = newFriendlyUrl,
                        OldUrl = page.FriendlyUrl,
                        ReferencedPages = referencePages.Select(referencePage => new PageConfirmChangeUrlItemModel
                        {
                            Id = referencePage.Id,
                            Title = referencePage.Title,
                            FriendlyUrl = referencePage.FriendlyUrl.ToPageFriendlyUrl()
                        }).ToList()
                    };

                    return model;
                }
            }

            return null;
        }

        /// <summary>
        /// Get all pages that reference page via friendly url in content
        /// </summary>
        /// <param name="friendlyUrl"></param>
        /// <returns></returns>
        public List<Page> GetReferencePagesViaLink(string friendlyUrl)
        {
            var pages = Fetch(p => p.Content.Contains(friendlyUrl)).ToList();
            var referencePages = pages.Where(p => p.Content.CheckContentUrl(friendlyUrl)).ToList();

            return referencePages;
        }

        /// <summary>
        /// Update related URL in content of referenced page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="oldUrl"></param>
        /// <param name="newUrl"></param>
        /// <returns></returns>
        private void UpdateContentOfReferencePage(Page page, string oldUrl, string newUrl)
        {
            //Convert new url to friendly url format
            newUrl = newUrl.ToPageFriendlyUrl();

            /*
             * Get all possible versions of old url
             */
            var oldUrls = new List<string>
            {
                oldUrl,
                oldUrl.ToPageFriendlyUrl(),
                oldUrl.ToAbsoluteUrl()
            };

            //Replace all posible urls
            foreach (var url in oldUrls)
            {
                page.Content = page.Content.ReplaceUrl(url, newUrl);
            }

            //Update page content
            Update(page);
        }

        #endregion

        #region Details

        /// <summary>
        /// Get page detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PageDetailModel GetPageDetailModel(int id)
        {
            var page = GetById(id);
            return page != null ? new PageDetailModel(page) : null;
        }

        /// <summary>
        /// Update page data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdatePageData(XEditableModel model)
        {
            var page = GetById(model.Pk);
            if (page != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (PageManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new PageManageModel(page);
                    manageModel.SetProperty(model.Name, value);

                    var validationResults = manageModel.ValidateModel();

                    if (validationResults.Any())
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = validationResults.BuildValidationMessages()
                        };
                    }

                    #endregion

                    page.SetProperty(model.Name, value);
                    var response = Update(page);
                    return response.SetMessage(response.Success
                        ? T("Page_Message_UpdatePageInfoSuccessfully")
                        : T("Page_Message_UpdatePageInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Page_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("Page_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}