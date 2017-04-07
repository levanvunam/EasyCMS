using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.EventCategories;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.EventCategories;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class EventCategoriesController : BackendController
    {
        private readonly IEventCategoryService _eventCategoryService;
        public EventCategoriesController(IEventCategoryService eventCategoryService)
        {
            _eventCategoryService = eventCategoryService;
        }

        #region Grid

        [AdministratorNavigation("Event_Categories", "Event_Holder", "Event_Categories", "fa-list-ol", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int? eventId)
        {
            return JsonConvert.SerializeObject(_eventCategoryService.SearchEventCategories(si, eventId));
        }

        /// <summary>
        /// Export event categories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, int? eventId)
        {
            var workbook = _eventCategoryService.Exports(si, gridExportMode, eventId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "EventCategories.xls");
        }

        #endregion

        #region Create

        [AdministratorNavigation("Event_Category_Create", "Event_Categories", "Event_Category_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _eventCategoryService.GetEventCategoryManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(EventCategoryManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _eventCategoryService.SaveEventCategory(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var id = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("Event_Category_Edit", "Event_Categories", "Event_Category_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _eventCategoryService.GetEventCategoryManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("EventCategory_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(EventCategoryManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _eventCategoryService.SaveEventCategory(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            if (!string.IsNullOrEmpty(returnUrl))
                                return Redirect(returnUrl);
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = model.Id, returnUrl });
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
            var model = _eventCategoryService.GetEventCategoryManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("EventCategory_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(EventCategoryManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _eventCategoryService.SaveEventCategory(model);
                SetResponseMessage(response);
                if (response.Success)
                {

                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox", new CloseFancyBoxViewModel
                            {
                                IsReload = false,
                                ReturnUrl = string.Empty
                            });
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("PopupEdit", new { id = model.Id });
                    }
                }
            }
            SetupPopupAction();
            return View(model);
        }

        #endregion

        #region Details

        [AdministratorNavigation("Event_Category_Details", "Event_Categories", "Event_Category_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _eventCategoryService.GetEventCategoryDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("EventCategory_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateEventCategoryData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_eventCategoryService.UpdateEventCategoryData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteEventCategory(int id)
        {
            return Json(_eventCategoryService.DeleteEventCategory(id));
        }

        #endregion

        #endregion

        [HttpPost]
        public JsonResult DeleteEventEventCategoryMapping(int eventId, int categoryId)
        {
            return Json(_eventCategoryService.DeleteEventEventCategoryMapping(eventId, categoryId));
        }
    }
}
