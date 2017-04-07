using System;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Reflection;
using Ez.Framework.Utilities.Time;
using Ez.Framework.Utilities.Web;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.LinkTrackers;
using EzCMS.Core.Models.LinkTrackers.MonthlyClickThrough;
using EzCMS.Core.Services.LinkTrackerClicks;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.LinkTrackers
{
    public class LinkTrackerService : ServiceHelper, ILinkTrackerService
    {
        private readonly ILinkTrackerClickService _linkTrackerClickService;
        private readonly IRepository<LinkTracker> _linkTrackerRepository;

        public LinkTrackerService(IRepository<LinkTracker> linkTrackerRepository,
            ILinkTrackerClickService linkTrackerClickService)
        {
            _linkTrackerRepository = linkTrackerRepository;
            _linkTrackerClickService = linkTrackerClickService;
        }

        #region Validation

        /// <summary>
        /// Check if link tracker exists
        /// </summary>
        /// <param name="id">the link tracker id</param>
        /// <param name="name">the link tracker name</param>
        /// <returns></returns>
        public bool IsNameExisted(int? id, string name)
        {
            return Fetch(u => u.Name.Equals(name) && u.Id != id).Any();
        }

        #endregion

        #region Link Tracker action

        /// <summary>
        /// Handle link tracker actions
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel TriggerLinkTrackerAction(int id)
        {
            var linkTracker = GetById(id);

            if (linkTracker != null)
            {
                var now = DateTime.UtcNow;

                var userAgentInformation = HttpContext.Current.GetUserAgentInformationFromRequest();

                var linkTrackerClick = new LinkTrackerClick
                {
                    IpAddress = userAgentInformation.IpAddress,
                    Platform = userAgentInformation.Platform,
                    OsVersion = userAgentInformation.OsVersion,
                    BrowserType = userAgentInformation.BrowserType,
                    BrowserName = userAgentInformation.BrowserName,
                    BrowserVersion = userAgentInformation.BrowserVersion,
                    MajorVersion = userAgentInformation.MajorVersion,
                    MinorVersion = userAgentInformation.MinorVersion,
                    IsBeta = userAgentInformation.IsBeta,
                    IsCrawler = userAgentInformation.IsCrawler,
                    IsAOL = userAgentInformation.IsAOL,
                    IsWin16 = userAgentInformation.IsWin16,
                    IsWin32 = userAgentInformation.IsWin32,
                    SupportsFrames = userAgentInformation.SupportsFrames,
                    SupportsTables = userAgentInformation.SupportsTables,
                    SupportsCookies = userAgentInformation.SupportsCookies,
                    SupportsVBScript = userAgentInformation.SupportsVBScript,
                    SupportsJavaScript = userAgentInformation.SupportsJavaScript,
                    SupportsJavaApplets = userAgentInformation.SupportsJavaApplets,
                    SupportsActiveXControls = userAgentInformation.SupportsActiveXControls,
                    JavaScriptVersion = userAgentInformation.JavaScriptVersion,
                    LinkTrackerId = linkTracker.Id
                };

                // Get the url
                var url = UrlUtilities.GenerateUrl(HttpContext.Current.Request.RequestContext, "LinkTrackers", "Index",
                    new {area = "", id = linkTracker.Id}, true);

                // Check cookie to prevent multiple count
                var cookieName = url.ToIdStringByHash();
                var linkTrackerCookie = HttpContext.Current.Request.Cookies[cookieName];
                if (linkTracker.IsAllowMultipleClick || linkTrackerCookie == null)
                {
                    // Add new cookie
                    linkTrackerCookie = new HttpCookie(cookieName)
                    {
                        Value = true.ToString(),
                        Expires = now.AddDays(EzCMSContants.LinkTrackerCookieExpireDay)
                    };
                    HttpContext.Current.Response.Cookies.Add(linkTrackerCookie);

                    _linkTrackerClickService.AddLinkTrackerClick(linkTrackerClick);
                }

                // Parse link tracker to url
                if (linkTracker.PageId.HasValue)
                {
                    return new ResponseModel
                    {
                        Success = true,
                        Data = linkTracker.Page.FriendlyUrl.ToPageFriendlyUrl(linkTracker.Page.IsHomePage)
                    };
                }

                return new ResponseModel
                {
                    Success = true,
                    Data = linkTracker.RedirectUrl
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("LinkTracker_Message_InvalidLinkTracker")
            };
        }

        #endregion

        #region Base

        public IQueryable<LinkTracker> GetAll()
        {
            return _linkTrackerRepository.GetAll();
        }

        public IQueryable<LinkTracker> Fetch(Expression<Func<LinkTracker, bool>> expression)
        {
            return _linkTrackerRepository.Fetch(expression);
        }

        public LinkTracker FetchFirst(Expression<Func<LinkTracker, bool>> expression)
        {
            return _linkTrackerRepository.FetchFirst(expression);
        }

        public LinkTracker GetById(object id)
        {
            return _linkTrackerRepository.GetById(id);
        }

        internal ResponseModel Insert(LinkTracker linkTracker)
        {
            return _linkTrackerRepository.Insert(linkTracker);
        }

        internal ResponseModel Update(LinkTracker linkTracker)
        {
            return _linkTrackerRepository.Update(linkTracker);
        }

        internal ResponseModel Delete(LinkTracker linkTracker)
        {
            return _linkTrackerRepository.Delete(linkTracker);
        }

        internal ResponseModel Delete(object id)
        {
            return _linkTrackerRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _linkTrackerRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid

        /// <summary>
        /// Search the link trackers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchLinkTrackers(JqSearchIn si, LinkTrackerSearchModel searchModel)
        {
            var data = SearchLinkTrackers(searchModel);

            var linkTrackers = Maps(data, searchModel);

            return si.Search(linkTrackers);
        }

        /// <summary>
        /// Export link trackers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="searchModel"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, LinkTrackerSearchModel searchModel)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchLinkTrackers(searchModel);

            var linkTrackers = Maps(data, searchModel);

            var exportData = si.Export(linkTrackers, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Search the link trackers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchLinkTrackersByPage(JqSearchIn si, int pageId)
        {
            var data = SearchLinkTrackers(pageId);

            var linkTrackers = Maps(data, new LinkTrackerSearchModel());

            return si.Search(linkTrackers);
        }

        /// <summary>
        /// Export link trackers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public HSSFWorkbook ExportsLinkTrackersByPage(JqSearchIn si, GridExportMode gridExportMode, int pageId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchLinkTrackers(pageId);

            var linkTrackers = Maps(data, new LinkTrackerSearchModel());

            var exportData = si.Export(linkTrackers, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search link trackers
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        private IQueryable<LinkTracker> SearchLinkTrackers(LinkTrackerSearchModel searchModel)
        {
            return Fetch(l => string.IsNullOrEmpty(searchModel.Keyword)
                              ||
                              (!string.IsNullOrEmpty(l.Name) && l.Name.ToLower().Contains(searchModel.Keyword.ToLower()))
                              ||
                              (!string.IsNullOrEmpty(l.RedirectUrl) &&
                               l.RedirectUrl.ToLower().Contains(searchModel.Keyword.ToLower())));
        }

        /// <summary>
        /// Search link trackers
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        private IQueryable<LinkTracker> SearchLinkTrackers(int pageId)
        {
            return Fetch(linkTracker => linkTracker.PageId == pageId);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="linkTrackers"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        private IQueryable<LinkTrackerModel> Maps(IQueryable<LinkTracker> linkTrackers,
            LinkTrackerSearchModel searchModel)
        {
            var trackerLink = UrlUtilities.GenerateUrl(HttpContext.Current.Request.RequestContext, "SiteApi",
                "LinkTracker", new {area = ""}, true);

            // Caculate the end date from date to
            DateTime? dateTo = null;
            if (searchModel.DateTo.HasValue) dateTo = searchModel.DateTo.Value.ToEndDate();

            return linkTrackers.Select(l => new LinkTrackerModel
            {
                Id = l.Id,
                Name = l.Name,
                TrackerLink = trackerLink + "?id=" + SqlFunctions.StringConvert((double) l.Id).Trim(),
                IsAllowMultipleClick = l.IsAllowMultipleClick,
                RedirectUrl = l.RedirectUrl ?? string.Empty,
                PageId = l.PageId,
                PageTitle = l.PageId.HasValue ? l.Page.Title : string.Empty,
                ClickCount = l.LinkTrackerClicks
                    .Count(lc => (!searchModel.DateFrom.HasValue || lc.Created >= searchModel.DateFrom.Value) &&
                                 (!dateTo.HasValue || lc.Created <= dateTo)),
                RecordOrder = l.RecordOrder,
                Created = l.Created,
                CreatedBy = l.CreatedBy,
                LastUpdate = l.LastUpdate,
                LastUpdateBy = l.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get link tracker detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LinkTrackerDetailModel GetLinkTrackerDetailModel(int? id = null)
        {
            var linkTracker = GetById(id);
            return linkTracker != null ? new LinkTrackerDetailModel(linkTracker) : null;
        }

        /// <summary>
        /// Get link tracker manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LinkTrackerManageModel GetLinkTrackerManageModel(int? id = null)
        {
            var linkTracker = GetById(id);
            return linkTracker != null ? new LinkTrackerManageModel(linkTracker) : new LinkTrackerManageModel();
        }

        /// <summary>
        /// Save link tracker
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveLinkTracker(LinkTrackerManageModel model)
        {
            ResponseModel response;

            var linkTracker = GetById(model.Id);
            if (linkTracker != null)
            {
                linkTracker.Name = model.Name;
                linkTracker.IsAllowMultipleClick = model.IsAllowMultipleClick;
                linkTracker.RedirectUrl = model.RedirectUrl;
                linkTracker.PageId = model.PageId;

                response = Update(linkTracker);
                response.SetMessage(response.Success
                    ? T("LinkTracker_Message_UpdateSuccessfully")
                    : T("LinkTracker_Message_UpdateFailure"));
            }
            else
            {
                Mapper.CreateMap<LinkTrackerManageModel, LinkTracker>();
                linkTracker = Mapper.Map<LinkTrackerManageModel, LinkTracker>(model);

                response = Insert(linkTracker);
                response.SetMessage(response.Success
                    ? T("LinkTracker_Message_CreateSuccessfully")
                    : T("LinkTracker_Message_CreateFailure"));
            }

            return response;
        }

        /// <summary>
        /// Delete the link tracker
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteLinkTracker(int id)
        {
            // Stop deletion if click found
            var linkTracker = GetById(id);
            if (linkTracker != null)
            {
                if (linkTracker.LinkTrackerClicks.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("LinkTracker_Message_DeleteFailureBasedOnRelatedClicks")
                    };
                }

                // Delete the link tracker
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("LinkTracker_Message_DeleteSuccessfully")
                    : T("LinkTracker_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("LinkTracker_Message_DeleteSuccessfully")
            };
        }

        /// <summary>
        /// Update link tracker data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateLinkTrackerData(XEditableModel model)
        {
            var linkTracker = GetById(model.Pk);
            if (linkTracker != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (LinkTrackerManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new LinkTrackerManageModel(linkTracker);
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

                    linkTracker.SetProperty(model.Name, value);

                    var response = Update(linkTracker);
                    return response.SetMessage(response.Success
                        ? T("LinkTracker_Message_UpdateLinkInfoSuccessfully")
                        : T("LinkTracker_Message_UpdateLinkInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("LinkTracker_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("LinkTracker_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Monthly Click Through

        /// <summary>
        /// Search the link trackers for monthly click through
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchLinkTrackers(JqSearchIn si, LinkTrackerMonthlyClickThroughSearchModel searchModel)
        {
            var data = SearchLinkTrackers(searchModel);
            var linkTrackers = Maps(data, searchModel);
            return si.Search(linkTrackers);
        }

        /// <summary>
        /// Export link trackers for monthly click through
        /// </summary>
        /// <returns></returns>
        public HSSFWorkbook Exports(LinkTrackerMonthlyClickThroughSearchModel searchModel)
        {
            var data = SearchLinkTrackers(searchModel);
            var linkTrackers = Maps(data, searchModel);
            return ExcelUtilities.CreateWorkBook(linkTrackers);
        }

        #region Private Methods

        /// <summary>
        /// Search link trackers for monthly click through
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        private IQueryable<LinkTracker> SearchLinkTrackers(LinkTrackerMonthlyClickThroughSearchModel searchModel)
        {
            return Fetch(l => string.IsNullOrEmpty(searchModel.Keyword)
                              ||
                              (!string.IsNullOrEmpty(l.Name) && l.Name.ToLower().Contains(searchModel.Keyword.ToLower()))
                              ||
                              (!string.IsNullOrEmpty(l.RedirectUrl) &&
                               l.RedirectUrl.ToLower().Contains(searchModel.Keyword.ToLower())));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="linkTrackers"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        private IQueryable<LinkTrackerMonthlyClickThroughModel> Maps(IQueryable<LinkTracker> linkTrackers,
            LinkTrackerMonthlyClickThroughSearchModel searchModel)
        {
            if (searchModel.Year.HasValue)
            {
                return linkTrackers.Select(l => new LinkTrackerMonthlyClickThroughModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    January =
                        l.LinkTrackerClicks.Count(
                            lc => lc.Created.Month == 1 && lc.Created.Year == searchModel.Year.Value),
                    February =
                        l.LinkTrackerClicks.Count(
                            lc => lc.Created.Month == 2 && lc.Created.Year == searchModel.Year.Value),
                    March =
                        l.LinkTrackerClicks.Count(
                            lc => lc.Created.Month == 3 && lc.Created.Year == searchModel.Year.Value),
                    April =
                        l.LinkTrackerClicks.Count(
                            lc => lc.Created.Month == 4 && lc.Created.Year == searchModel.Year.Value),
                    May =
                        l.LinkTrackerClicks.Count(
                            lc => lc.Created.Month == 5 && lc.Created.Year == searchModel.Year.Value),
                    June =
                        l.LinkTrackerClicks.Count(
                            lc => lc.Created.Month == 6 && lc.Created.Year == searchModel.Year.Value),
                    July =
                        l.LinkTrackerClicks.Count(
                            lc => lc.Created.Month == 7 && lc.Created.Year == searchModel.Year.Value),
                    August =
                        l.LinkTrackerClicks.Count(
                            lc => lc.Created.Month == 8 && lc.Created.Year == searchModel.Year.Value),
                    September =
                        l.LinkTrackerClicks.Count(
                            lc => lc.Created.Month == 9 && lc.Created.Year == searchModel.Year.Value),
                    October =
                        l.LinkTrackerClicks.Count(
                            lc => lc.Created.Month == 10 && lc.Created.Year == searchModel.Year.Value),
                    November =
                        l.LinkTrackerClicks.Count(
                            lc => lc.Created.Month == 11 && lc.Created.Year == searchModel.Year.Value),
                    December =
                        l.LinkTrackerClicks.Count(
                            lc => lc.Created.Month == 12 && lc.Created.Year == searchModel.Year.Value)
                });
            }

            return linkTrackers.Select(l => new LinkTrackerMonthlyClickThroughModel
            {
                Id = l.Id,
                Name = l.Name,
                January = l.LinkTrackerClicks.Count(lc => lc.Created.Month == 1),
                February = l.LinkTrackerClicks.Count(lc => lc.Created.Month == 2),
                March = l.LinkTrackerClicks.Count(lc => lc.Created.Month == 3),
                April = l.LinkTrackerClicks.Count(lc => lc.Created.Month == 4),
                May = l.LinkTrackerClicks.Count(lc => lc.Created.Month == 5),
                June = l.LinkTrackerClicks.Count(lc => lc.Created.Month == 6),
                July = l.LinkTrackerClicks.Count(lc => lc.Created.Month == 7),
                August = l.LinkTrackerClicks.Count(lc => lc.Created.Month == 8),
                September = l.LinkTrackerClicks.Count(lc => lc.Created.Month == 9),
                October = l.LinkTrackerClicks.Count(lc => lc.Created.Month == 10),
                November = l.LinkTrackerClicks.Count(lc => lc.Created.Month == 11),
                December = l.LinkTrackerClicks.Count(lc => lc.Created.Month == 12)
            });
        }

        #endregion

        #endregion
    }
}