using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.EventSchedules;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.EventSchedules;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize]
    public class EventSchedulesController : BackendController
    {
        private readonly IEventScheduleService _eventScheduleService;
        public EventSchedulesController(IEventScheduleService eventScheduleService)
        {
            _eventScheduleService = eventScheduleService;
        }

        [AdministratorNavigation("Event_Schedules", "Event_Holder", "Event_Schedules", "fa-calendar-o", 30)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int? eventId)
        {
            return JsonConvert.SerializeObject(_eventScheduleService.SearchEventSchedules(si, eventId));
        }

        /// <summary>
        /// Export event schedules
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, int? eventId)
        {
            var workbook = _eventScheduleService.Exports(si, gridExportMode, eventId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "EventSchedules.xls");
        }

        #region Create

        [AdministratorNavigation("Event_Schedule_Create", "Event_Schedules", "Event_Schedule_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var eventSchedule = _eventScheduleService.GetEventScheduleManageModel();
            return View(eventSchedule);
        }

        [HttpPost]
        public ActionResult Create(EventScheduleManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _eventScheduleService.SaveEventSchedule(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var eventScheduleId = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("Edit", new { id = eventScheduleId });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("Event_Schedule_Edit", "Event_Schedules", "Event_Schedule_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _eventScheduleService.GetEventScheduleManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("EventSchedule_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EventScheduleManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _eventScheduleService.SaveEventSchedule(model);
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
            var model = _eventScheduleService.GetEventScheduleManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("EventSchedule_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(EventScheduleManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _eventScheduleService.SaveEventSchedule(model);
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

        [AdministratorNavigation("Event_Schedule_Details", "Event_Schedules", "Event_Schedule_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _eventScheduleService.GetEventScheduleDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("EventSchedule_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateEventScheduleData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_eventScheduleService.UpdateEventScheduleData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteEventSchedule(int id)
        {
            return Json(_eventScheduleService.DeleteEventSchedule(id));
        }

        #endregion

        #endregion
    }
}
