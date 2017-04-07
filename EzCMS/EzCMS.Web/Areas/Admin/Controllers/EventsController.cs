using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Events;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.Events;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize]
    public class EventsController : BackendController
    {
        private readonly IEventService _eventService;
        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [AdministratorNavigation("Event_Holder", "Module_Holder", "Event_Holder", "fa-bell", 30, true, true)]
        [AdministratorNavigation("Events", "Event_Holder", "Events", "fa-users", 10)]
        public ActionResult Index()
        {
            var model = new EventSearchModel();
            return View(model);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, EventSearchModel model)
        {
            return JsonConvert.SerializeObject(_eventService.SearchEvents(si, model));
        }

        /// <summary>
        /// Export events
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, EventSearchModel model)
        {
            var workbook = _eventService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Events.xls");
        }

        #region Create

        [AdministratorNavigation("Event_Create", "Events", "Event_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _eventService.GetEventCreateModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(EventCreateModel model, SubmitType submit)
        {
            ModelState.Remove("EventSchedule.EventId");
            if (ModelState.IsValid)
            {
                var response = _eventService.SaveEvent(model);
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

        [AdministratorNavigation("Event_Edit", "Events", "Event_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _eventService.GetEventManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Event_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(EventManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _eventService.SaveEvent(model);
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

        #region Popup Create

        public ActionResult PopupCreate()
        {
            SetupPopupAction();
            var model = _eventService.GetEventCreateModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupCreate(EventCreateModel model, SubmitType submit)
        {
            ModelState.Remove("EventSchedule.EventId");
            if (ModelState.IsValid)
            {
                var response = _eventService.SaveEvent(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var id = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox",
                                new CloseFancyBoxViewModel
                                {
                                    IsReload = true
                                });
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("PopupEdit", new { id });
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
            var model = _eventService.GetEventManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Event_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(EventManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _eventService.SaveEvent(model);
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

        [AdministratorNavigation("Event_Details", "Events", "Event_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _eventService.GetEventDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Event_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateEventData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_eventService.UpdateEventData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteEvent(int id)
        {
            return Json(_eventService.DeleteEvent(id));
        }

        #endregion

        #endregion
    }
}
