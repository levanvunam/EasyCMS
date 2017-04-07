using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.LinkTrackers;
using EzCMS.Core.Models.LinkTrackers.MonthlyClickThrough;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.LinkTrackers;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize]
    public class LinkTrackersController : BackendController
    {
        private readonly ILinkTrackerService _linkTrackerService;
        public LinkTrackersController(ILinkTrackerService linkTrackerService)
        {
            _linkTrackerService = linkTrackerService;
        }

        [AdministratorNavigation("Link_Tracker_Holder", "Link_Holder", "Link_Tracker_Holder", "fa-link", 50, true, true)]
        [AdministratorNavigation("Link_Trackers", "Link_Tracker_Holder", "Link_Trackers", "fa-link", 10)]
        public ActionResult Index()
        {
            var searchModel = new LinkTrackerSearchModel();
            return View(searchModel);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, LinkTrackerSearchModel searchModel)
        {
            return JsonConvert.SerializeObject(_linkTrackerService.SearchLinkTrackers(si, searchModel));
        }

        /// <summary>
        /// Export link trackers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, LinkTrackerSearchModel searchModel)
        {
            var workbook = _linkTrackerService.Exports(si, gridExportMode, searchModel);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "LinkTrackers.xls");
        }

        [HttpGet]
        public string _AjaxBindingByPage(JqSearchIn si, int pageId)
        {
            return JsonConvert.SerializeObject(_linkTrackerService.SearchLinkTrackersByPage(si, pageId));
        }

        /// <summary>
        /// Export link trackers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ActionResult ExportsLinkTrackersByPage(JqSearchIn si, GridExportMode gridExportMode, int pageId)
        {
            var workbook = _linkTrackerService.ExportsLinkTrackersByPage(si, gridExportMode, pageId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "LinkTrackers.xls");
        }

        #region Create

        [AdministratorNavigation("Link_Tracker_Create", "Link_Tracker", "Link_Tracker_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _linkTrackerService.GetLinkTrackerManageModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(LinkTrackerManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _linkTrackerService.SaveLinkTracker(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var id = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("Edit", new { id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("Link_Tracker_Edit", "Link_Tracker", "Link_Tracker_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _linkTrackerService.GetLinkTrackerManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("LinkTracker_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(LinkTrackerManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _linkTrackerService.SaveLinkTracker(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            if (!string.IsNullOrEmpty(returnUrl))
                                return Redirect(returnUrl);
                            return RedirectToAction("Index");
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("Edit", new { id = model.Id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Popup Edit

        public ActionResult PopupEdit(int id)
        {
            SetupPopupAction();
            var model = _linkTrackerService.GetLinkTrackerManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("LinkTracker_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult PopupEdit(LinkTrackerManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _linkTrackerService.SaveLinkTracker(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox",
                                new CloseFancyBoxViewModel
                                {
                                    IsReload = true
                                });
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("PopupEdit", new { id = model.Id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Details

        [AdministratorNavigation("Link_Tracker_Details", "Link_Tracker", "Link_Tracker_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _linkTrackerService.GetLinkTrackerDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("LinkTracker_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateLinkTrackerData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_linkTrackerService.UpdateLinkTrackerData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteLinkTracker(int id)
        {
            return Json(_linkTrackerService.DeleteLinkTracker(id));
        }

        #endregion

        #endregion

        #region Monthly Click Through

        [AdministratorNavigation("Link_Trackers_Monthly_Click_Through", "Link_Tracker_Holder", "Link_Trackers_Monthly_Click_Through", "fa-external-link", 30)]
        public ActionResult MonthlyClickThrough()
        {
            var searchModel = new LinkTrackerMonthlyClickThroughSearchModel();
            return View(searchModel);
        }

        [HttpGet]
        public string _LinkTrackerMonthlyClickThroughAjaxBinding(JqSearchIn si, LinkTrackerMonthlyClickThroughSearchModel searchModel)
        {
            return JsonConvert.SerializeObject(_linkTrackerService.SearchLinkTrackers(si, searchModel));
        }

        public ActionResult LinkTrackerMonthlyClickThroughExports(LinkTrackerMonthlyClickThroughSearchModel searchModel)
        {
            var workbook = _linkTrackerService.Exports(searchModel);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "LinkTrackersMonthlyClickThrough.xls");
        }

        #endregion
    }
}
