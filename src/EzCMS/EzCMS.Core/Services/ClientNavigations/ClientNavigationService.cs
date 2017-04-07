using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.ClientNavigations;
using EzCMS.Core.Models.ClientNavigations.Widgets;
using EzCMS.Core.Models.Pages.Sitemap;
using EzCMS.Core.Models.UserGroups;
using EzCMS.Core.Services.Tree;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.ClientNavigations;
using EzCMS.Entity.Repositories.Pages;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.ClientNavigations
{
    public class ClientNavigationService : ServiceHelper, IClientNavigationService
    {
        private readonly IClientNavigationRepository _clientNavigationRepository;
        private readonly IPageRepository _pageRepository;

        public ClientNavigationService(IClientNavigationRepository clientNavigationRepository,
            IPageRepository pageRepository)
        {
            _clientNavigationRepository = clientNavigationRepository;
            _pageRepository = pageRepository;
        }

        /// <summary>
        /// Get possible parent ClientNavigation
        /// </summary>
        /// <param name="id">the current ClientNavigation id</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetPossibleParents(int? id = null)
        {
            var clientNavigations = GetAll();
            int? parentId = null;
            var clientNavigation = GetById(id);
            if (clientNavigation != null)
            {
                parentId = clientNavigation.ParentId;
                clientNavigations = _clientNavigationRepository.GetPossibleParents(clientNavigation);
            }
            var data = clientNavigations.Select(m => new HierarchyDropdownModel
            {
                Id = m.Id,
                Name = m.PageId.HasValue ? m.Page.Title : m.Title,
                Hierarchy = m.Hierarchy,
                RecordOrder =
                    m.PageId.HasValue ? m.Page.RecordOrder*EzCMSContants.OrderMultipleTimes : m.RecordOrder,
                Selected = parentId.HasValue && parentId.Value == m.Id
            }).ToList();
            return _clientNavigationRepository.BuildSelectList(data);
        }

        /// <summary>
        /// Get clinet Navigation by page id
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ClientNavigation GetByPage(int? pageId)
        {
            return FetchFirst(m => pageId.HasValue && m.PageId == pageId);
        }

        #region Base

        public IQueryable<ClientNavigation> GetAll()
        {
            return _clientNavigationRepository.GetAll();
        }

        public IQueryable<ClientNavigation> Fetch(Expression<Func<ClientNavigation, bool>> expression)
        {
            return _clientNavigationRepository.Fetch(expression);
        }

        public ClientNavigation FetchFirst(Expression<Func<ClientNavigation, bool>> expression)
        {
            return _clientNavigationRepository.FetchFirst(expression);
        }

        public ClientNavigation GetById(object id)
        {
            return _clientNavigationRepository.GetById(id);
        }

        internal ResponseModel Insert(ClientNavigation clientNavigation)
        {
            return _clientNavigationRepository.Insert(clientNavigation);
        }

        internal ResponseModel Update(ClientNavigation clientNavigation)
        {
            return _clientNavigationRepository.Update(clientNavigation);
        }

        internal ResponseModel HierarchyUpdate(ClientNavigation clientNavigation)
        {
            return _clientNavigationRepository.HierarchyUpdate(clientNavigation);
        }

        internal ResponseModel HierarchyInsert(ClientNavigation clientNavigation)
        {
            return _clientNavigationRepository.HierarchyInsert(clientNavigation);
        }

        internal ResponseModel Delete(ClientNavigation clientNavigation)
        {
            return _clientNavigationRepository.Delete(clientNavigation);
        }

        internal ResponseModel Delete(object id)
        {
            return _clientNavigationRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _clientNavigationRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the client Navigations
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchClientNavigations(JqSearchIn si)
        {
            var data = GetAll();

            var clientNavigations = Maps(data);

            return si.Search(clientNavigations);
        }

        /// <summary>
        /// Export client Navigations
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var clientNavigations = Maps(data);

            var exportData = si.Export(clientNavigations, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="clientNavigations"></param>
        /// <returns></returns>
        private IQueryable<ClientNavigationModel> Maps(IQueryable<ClientNavigation> clientNavigations)
        {
            var selfTarget = CommonEnums.UrlTarget.Self.GetEnumName();
            return clientNavigations.Select(Navigation => new ClientNavigationModel
            {
                Id = Navigation.Id,
                PageId = Navigation.PageId,
                Url = Navigation.PageId.HasValue ? Navigation.Page.FriendlyUrl : Navigation.Url,
                UrlTarget = Navigation.PageId.HasValue ? selfTarget : Navigation.UrlTarget,
                Title = Navigation.PageId.HasValue ? Navigation.Page.Title : Navigation.Title,
                StartPublishingDate =
                    Navigation.PageId.HasValue ? Navigation.Page.StartPublishingDate : Navigation.StartPublishingDate,
                EndPublishingDate =
                    Navigation.PageId.HasValue ? Navigation.Page.EndPublishingDate : Navigation.EndPublishingDate,
                IsPageNavigation = Navigation.PageId.HasValue,
                Hierarchy = Navigation.Hierarchy,
                ParentId = Navigation.ParentId,
                ParentName =
                    Navigation.ParentId.HasValue
                        ? (Navigation.Parent.PageId.HasValue ? Navigation.Parent.Page.Title : Navigation.Parent.Title)
                        : string.Empty,
                RecordOrder =
                    Navigation.PageId.HasValue
                        ? Navigation.Page.RecordOrder*EzCMSContants.OrderMultipleTimes
                        : Navigation.RecordOrder,
                Created = Navigation.Created,
                CreatedBy = Navigation.CreatedBy,
                LastUpdate = Navigation.LastUpdate,
                LastUpdateBy = Navigation.LastUpdateBy
            });
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get client Navigation manage model by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClientNavigationManageModel GetClientNavigationManageModel(int? id = null)
        {
            var clientNavigation = GetById(id);
            return clientNavigation != null
                ? new ClientNavigationManageModel(clientNavigation)
                : new ClientNavigationManageModel();
        }

        /// <summary>
        /// Save client Navigation manage model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveClientNavigationManageModel(ClientNavigationManageModel model)
        {
            ClientNavigation relativeNavigation;
            ResponseModel response;
            var clientNavigation = GetById(model.Id);

            #region Edit Client Navigation

            if (clientNavigation != null)
            {
                clientNavigation.Title = model.Title;

                clientNavigation.IncludeInSiteNavigation = model.IncludeInSiteNavigation;
                clientNavigation.DisableNavigationCascade = model.DisableNavigationCascade;
                clientNavigation.StartPublishingDate = model.StartPublishingDate;
                clientNavigation.EndPublishingDate = model.EndPublishingDate;
                clientNavigation.ParentId = model.ParentId;
                clientNavigation.Url = model.Url;
                clientNavigation.UrlTarget = model.UrlTarget;

                //Get page record order
                relativeNavigation = GetById(model.RelativeNavigationId);
                if (relativeNavigation != null)
                {
                    if (model.Position == (int) ClientNavigationEnums.PositionEnums.Before)
                    {
                        clientNavigation.RecordOrder = relativeNavigation.PageId.HasValue
                            ? relativeNavigation.Page.RecordOrder*EzCMSContants.OrderMultipleTimes - 1
                            : relativeNavigation.RecordOrder - 1;
                    }
                    else
                    {
                        clientNavigation.RecordOrder = relativeNavigation.PageId.HasValue
                            ? relativeNavigation.Page.RecordOrder*EzCMSContants.OrderMultipleTimes + 1
                            : relativeNavigation.RecordOrder + 1;
                    }
                }

                response = HierarchyUpdate(clientNavigation);

                return response.SetMessage(response.Success
                    ? T("ClientNavigation_Message_UpdateSuccessfully")
                    : T("ClientNavigation_Message_UpdateFailure"));
            }

            #endregion

            Mapper.CreateMap<ClientNavigationManageModel, ClientNavigation>();
            clientNavigation = Mapper.Map<ClientNavigationManageModel, ClientNavigation>(model);

            //Get Navigation record order
            relativeNavigation = GetById(model.RelativeNavigationId);
            if (relativeNavigation != null)
            {
                if (model.Position == (int) ClientNavigationEnums.PositionEnums.Before)
                {
                    clientNavigation.RecordOrder = relativeNavigation.PageId.HasValue
                        ? relativeNavigation.Page.RecordOrder*EzCMSContants.OrderMultipleTimes - 1
                        : relativeNavigation.RecordOrder - 1;
                }
                else
                {
                    clientNavigation.RecordOrder = relativeNavigation.PageId.HasValue
                        ? relativeNavigation.Page.RecordOrder*EzCMSContants.OrderMultipleTimes + 1
                        : relativeNavigation.RecordOrder + 1;
                }
            }

            response = HierarchyInsert(clientNavigation);
            return response.SetMessage(response.Success
                ? T("ClientNavigation_Message_CreateSuccessfully")
                : T("ClientNavigation_Message_CreateFailure"));
        }

        /// <summary>
        /// Get ClientNavigation by parent id
        /// </summary>
        /// <param name="position"> </param>
        /// <param name="relativeClientNavigationId"> </param>
        /// <param name="clientNavigationId"> </param>
        /// <param name="parentId">the parent id</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetRelativeNavigations(out int position, out int relativeClientNavigationId,
            int? clientNavigationId = null, int? parentId = null)
        {
            position = (int) ClientNavigationEnums.PositionEnums.Before;
            relativeClientNavigationId = 0;
            var order = 0;
            var relativeClientNavigations =
                Fetch(
                    p => (!clientNavigationId.HasValue || p.Id != clientNavigationId) &&
                         (parentId.HasValue ? p.ParentId == parentId : p.ParentId == null))
                    .Select(p => new
                    {
                        p.Id,
                        Name = p.PageId.HasValue ? p.Page.Title : p.Title,
                        RecordOrder =
                            p.PageId.HasValue
                                ? p.Page.RecordOrder*EzCMSContants.OrderMultipleTimes
                                : p.RecordOrder
                    }).OrderBy(m => m.RecordOrder).ToList();
            var clientNavigation = GetById(clientNavigationId);

            if (clientNavigation != null)
            {
                order = clientNavigation.RecordOrder;
            }

            //Flag to check if current ClientNavigation is the bigest order in relative ClientNavigation list
            var flag = false;
            for (var i = 0; i < relativeClientNavigations.Count(); i++)
            {
                if (relativeClientNavigations[i].RecordOrder > order)
                {
                    relativeClientNavigationId = relativeClientNavigations[i].Id;
                    flag = true;
                    break;
                }
            }
            if (!flag && relativeClientNavigations.Any())
            {
                position = (int) ClientNavigationEnums.PositionEnums.After;
                relativeClientNavigationId = relativeClientNavigations.Last().Id;
            }
            var selectClientNavigationId = relativeClientNavigationId;
            return relativeClientNavigations.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString(CultureInfo.InvariantCulture),
                Selected = p.Id == selectClientNavigationId
            });
        }

        /// <summary>
        /// Get ClientNavigation by parent id
        /// </summary>
        /// <param name="NavigationId"> </param>
        /// <param name="parentId">the parent id</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetRelativeNavigations(int? NavigationId = null, int? parentId = null)
        {
            return Fetch(
                p =>
                    (!NavigationId.HasValue || p.Id != NavigationId) &&
                    (parentId.HasValue ? p.ParentId == parentId : p.ParentId == null))
                .OrderBy(p => p.RecordOrder).Select(p => new SelectListItem
                {
                    Text = p.PageId.HasValue ? p.Page.Title : p.Title,
                    Value = SqlFunctions.StringConvert((double) p.Id).Trim()
                });
        }

        /// <summary>
        /// Delete client Navigation by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteNavigation(int id)
        {
            var Navigation = GetById(id);

            if (Navigation != null)
            {
                if (Navigation.PageId.HasValue)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("ClientNavigation_Message_CannotDeletePageNavigation")
                    };
                }

                if (Navigation.ChildNavigations.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("ClientNavigation_Message_DeleteFailureBasedOnChildrenNavigations")
                    };
                }

                var response = Delete(Navigation);
                return response.SetMessage(response.Success
                    ? T("ClientNavigation_Message_DeleteSuccessfully")
                    : T("ClientNavigation_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("ClientNavigation_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Widgets

        #region Site map

        /// <summary>
        /// Generate site map
        /// </summary>
        /// <param name="pageRootId"></param>
        /// <returns></returns>
        public List<ITree<NavigationNodeModel>> GenerateSiteMap(int? pageRootId = null)
        {
            var root = GetByPage(pageRootId);

            var clientNavigations = root != null
                ? _clientNavigationRepository.GetHierarchies(root)
                : _clientNavigationRepository.GetAll();

            var models = clientNavigations.ToList().Select(Navigation => new NavigationNodeModel(Navigation));

            var result =
                HierachyTree<NavigationNodeModel>.CreateForest(models).OrderBy(i => i.Data.RecordOrder).ToList();

            //Setup the permissions
            var rootViewableGroupIds = GetViewableGroups(pageRootId);
            var rootEditableGroupIds = GetEditableGroups(pageRootId);
            foreach (var tree in result)
            {
                SetupGroupPermissions(tree, rootViewableGroupIds, rootEditableGroupIds, true);
            }

            var comparer =
                Comparer<NavigationNodeModel>.Create(
                    (i, j) => Comparer<int>.Default.Compare(i.RecordOrder, j.RecordOrder));

            //For page included in navigation, just need the view permission
            //For page which is not included in navigation, users need the edit permission to see it in the site map.
            result.RemoveAll(i => !i.Data.CanView || (!i.Data.CanEdit && !i.Data.IncludeInSiteNavigation));

            foreach (var tree in result)
            {
                tree.RemoveAll(i => !i.CanView || (!i.CanEdit && !i.IncludeInSiteNavigation));
                tree.OrderChildren(comparer);
            }

            return result.Cast<ITree<NavigationNodeModel>>().ToList();
        }

        /// <summary>
        /// Generate google site map from pages
        /// </summary>
        /// <returns></returns>
        public GoogleSiteMap GenerateGoogleSiteMap()
        {
            var requestContext = HttpContext.Current.Request;
            var currentUrl = requestContext.Url;

            string urlPrefix;
            if (requestContext.Url.IsDefaultPort)
            {
                urlPrefix = string.Format("{0}://{1}/", currentUrl.Scheme, currentUrl.Host);
            }
            else
            {
                urlPrefix = string.Format("{0}:{2}://{1}/", currentUrl.Scheme, currentUrl.Host, currentUrl.Port);
            }


            var urls =
                _pageRepository.Fetch(i => !i.PageSecurities.Any() && i.Content != null)
                    .AsEnumerable()
                    .Select(i => new GoogleSiteMapUrl
                    {
                        Location = i.IsHomePage ? urlPrefix : urlPrefix + i.FriendlyUrl,
                        LastModified =
                            i.LastUpdate.HasValue
                                ? i.LastUpdate.Value.ToString("yyyy-MM-dd")
                                : i.Created.ToString("yyyy-MM-dd"),

                        //Hard-code based on EzCMS6
                        ChangeFreq = "weekly",
                        Priority = i.ParentId == null || i.Parent.Content == null ? "1.0" : "0.5"
                    }).ToList();

            return new GoogleSiteMap
            {
                urls = urls
            };
        }

        #endregion

        #region Navigations

        /// <summary>
        /// Generate Navigation
        /// </summary>
        /// <param name="pageRootId"></param>
        /// <param name="includeRoot"></param>
        /// <returns></returns>
        public List<ITree<NavigationNodeModel>> GenerateNavigations(int? pageRootId = null, bool includeRoot = false)
        {
            var root = GetByPage(pageRootId);

            //Get all avail Navigations
            var clientNavigations = root != null
                ? _clientNavigationRepository.GetHierarchies(root, false)
                : _clientNavigationRepository.GetAll();


            //Remove disabled cascade Navigations and their children
            var disableNavigationCascadeNavigations =
                clientNavigations.Where(
                    m => m.PageId.HasValue ? m.Page.DisableNavigationCascade : m.DisableNavigationCascade)
                    .Select(m => m.Hierarchy)
                    .ToList();
            clientNavigations =
                clientNavigations.Where(
                    m =>
                        !disableNavigationCascadeNavigations.Any(
                            hierarchy => !m.Hierarchy.Equals(hierarchy) && m.Hierarchy.StartsWith(hierarchy)));

            //Remove hide from navigation cascade Navigation, not publishing pages, offline pages and their children
            var now = DateTime.UtcNow;
            var hidingNavigations = clientNavigations.Where(m =>

                //Not Include in site navigation page
                (m.PageId.HasValue ? !m.Page.IncludeInSiteNavigation : !m.IncludeInSiteNavigation)

                    //Not Include in site navigation page
                || (m.PageId.HasValue && m.Page.Status == PageEnums.PageStatus.Offline)

                    //Not publishing page
                ||
                ((m.PageId.HasValue ? m.Page.StartPublishingDate.HasValue : m.StartPublishingDate.HasValue) &&
                 (m.PageId.HasValue ? now < m.Page.StartPublishingDate : now < m.StartPublishingDate))
                ||
                ((m.PageId.HasValue ? m.Page.EndPublishingDate.HasValue : m.EndPublishingDate.HasValue) &&
                 (m.PageId.HasValue ? now > m.Page.EndPublishingDate : now > m.EndPublishingDate)))
                .Select(m => m.Hierarchy).ToList();
            clientNavigations =
                clientNavigations.Where(m => !hidingNavigations.Any(hierarchy => m.Hierarchy.StartsWith(hierarchy)));

            var NavigationModels =
                clientNavigations.ToList().Select(Navigation => new NavigationNodeModel(Navigation)).ToList();

            var result =
                HierachyTree<NavigationNodeModel>.CreateForest(NavigationModels)
                    .OrderBy(i => i.Data.RecordOrder)
                    .ToList();

            //Setup the permissions
            var rootViewableGroupIds = GetViewableGroups(pageRootId);
            var rootEditableGroupIds = GetEditableGroups(pageRootId);
            foreach (var tree in result)
            {
                SetupGroupPermissions(tree, rootViewableGroupIds, rootEditableGroupIds);
            }

            //Order all items
            var comparer =
                Comparer<NavigationNodeModel>.Create(
                    (i, j) => Comparer<int>.Default.Compare(i.RecordOrder, j.RecordOrder));

            foreach (var tree in result)
            {
                tree.OrderChildren(comparer);
            }

            // Add root Navigation if required
            if (includeRoot && root != null)
            {
                var rootNode = new NavigationNodeModel(root)
                {
                    ViewableGroups = rootViewableGroupIds,
                    EditableGroups = rootEditableGroupIds
                };

                result.Insert(0, new HierachyTree<NavigationNodeModel>(rootNode));
            }

            return result.Cast<ITree<NavigationNodeModel>>().ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get all viewable group of page
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        private List<GroupItem> GetViewableGroups(int? pageId)
        {
            var Navigation = GetByPage(pageId);
            if (Navigation == null || !Navigation.PageId.HasValue)
            {
                return new List<GroupItem>();
            }
            var page = Navigation.Page;

            if (page.PageSecurities.Any())
            {
                return page.PageSecurities.Where(s => s.CanView)
                    .Select(s => new GroupItem
                    {
                        Id = s.GroupId,
                        Name = s.UserGroup.Name
                    }).ToList();
            }

            //Get all parent pages to check cascade secure
            var allParentPages = _pageRepository.GetParents(page, false, false).ToList();

            if (allParentPages.Any())
            {
                foreach (var item in allParentPages)
                {
                    if (item.PageSecurities.Any())
                    {
                        return page.PageSecurities.Where(s => s.CanView)
                            .Select(s => new GroupItem
                            {
                                Id = s.GroupId,
                                Name = s.UserGroup.Name
                            }).ToList();
                    }
                }
            }

            return new List<GroupItem>();
        }

        /// <summary>
        /// Get all editable groups of Navigation
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        private List<GroupItem> GetEditableGroups(int? pageId)
        {
            var Navigation = GetByPage(pageId);
            if (Navigation == null || !Navigation.PageId.HasValue)
            {
                return new List<GroupItem>();
            }
            var page = Navigation.Page;

            if (page.PageSecurities.Any())
            {
                return page.PageSecurities.Where(s => s.CanEdit)
                    .Select(s => new GroupItem
                    {
                        Id = s.GroupId,
                        Name = s.UserGroup.Name
                    }).ToList();
            }

            //Get all parent pages to check cascade secure
            var allParentPages = _pageRepository.GetParents(page, false, false).ToList();

            if (allParentPages.Any())
            {
                foreach (var item in allParentPages)
                {
                    if (item.PageSecurities.Any())
                    {
                        return page.PageSecurities.Where(s => s.CanEdit)
                            .Select(s => new GroupItem
                            {
                                Id = s.GroupId,
                                Name = s.UserGroup.Name
                            }).ToList();
                    }
                }
            }

            return new List<GroupItem>();
        }

        /// <summary>
        /// Setup group permission
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="viewableGroups"></param>
        /// <param name="editableGroups"></param>
        /// <param name="setupCanViewAndEdit"></param>
        private void SetupGroupPermissions(ITree<NavigationNodeModel> tree, List<GroupItem> viewableGroups,
            List<GroupItem> editableGroups, bool setupCanViewAndEdit = false)
        {
            //Setup root of tree
            if (viewableGroups.Any() || editableGroups.Any())
            {
                if (!tree.Data.ViewableGroups.Any() && !tree.Data.EditableGroups.Any())
                {
                    tree.Data.ViewableGroups = viewableGroups;
                    tree.Data.EditableGroups = editableGroups;
                }
            }

            if (setupCanViewAndEdit)
            {
                if (WorkContext.CurrentUser != null && WorkContext.CurrentUser.IsSystemAdministrator)
                {
                    tree.Data.CanView = true;
                    tree.Data.CanEdit = true;
                }
                else
                {
                    // Check viewable
                    if (tree.Data.ViewableGroups.Any())
                    {
                        if (WorkContext.CurrentUser != null &&
                            WorkContext.CurrentUser.GroupIds.Any(g => tree.Data.ViewableGroups.Any(vg => vg.Id == g)))
                        {
                            tree.Data.CanView = true;
                        }
                    }
                    else
                    {
                        tree.Data.CanView = true;
                    }

                    // Check editable
                    if (tree.Data.EditableGroups.Any())
                    {
                        if (WorkContext.CurrentUser != null &&
                            WorkContext.CurrentUser.GroupIds.Any(g => tree.Data.EditableGroups.Any(vg => vg.Id == g)))
                        {
                            tree.Data.CanEdit = true;
                        }
                    }
                    else
                    {
                        tree.Data.CanEdit = false;
                    }
                }
            }

            //Setup all children
            foreach (var node in tree.Children)
            {
                SetupGroupPermissions(node, tree.Data.ViewableGroups, tree.Data.EditableGroups, setupCanViewAndEdit);
            }
        }

        #endregion

        #endregion
    }
}