using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.NotificationTemplates;
using EzCMS.Core.Services.Widgets;
using EzCMS.Core.Services.NotificationTemplates;
using EzCMS.Entity.Core.Enums;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize]
    public class NotificationTemplatesController : BackendController
    {
        private readonly INotificationTemplateService _notificationTemplateService;
        private readonly IWidgetService _widgetService;
        public NotificationTemplatesController(INotificationTemplateService notificationTemplateService, IWidgetService widgetService)
        {
            _notificationTemplateService = notificationTemplateService;
            _widgetService = widgetService;
        }

        [AdministratorNavigation("Notification_Templates", "Notification_Holder", "Notification_Templates", "fa-th-list", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_notificationTemplateService.SearchNotificationTemplates(si));
        }

        /// <summary>
        /// Export notification templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _notificationTemplateService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "NotificationTemplates.xls");
        }

        /// <summary>
        /// Generate Property Dropdown from NotificationModule
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GeneratePropertyDropdown(NotificationEnums.NotificationModule module)
        {
            var type = _notificationTemplateService.GetNotificationEmailModelAssemblyName(module);

            var model = _widgetService.GetPropertyListFromType(type);
            return Json(new ResponseModel
            {
                Success = true,
                Data = RenderPartialViewToString("Partials/_PropertyDropdown", model)
            });
        }

        #region Create		
		
		[AdministratorNavigation("Notification_Template_Create", "Notification_Templates", "Notification_Template_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var template = _notificationTemplateService.GetNotificationTemplateManageModel();
            return View(template);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(NotificationTemplateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _notificationTemplateService.SaveNotificationTemplate(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var templateId = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("Edit", new { id = templateId });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Edit

        [AdministratorNavigation("Notification_Template_Edit", "Notification_Templates", "Notification_Template_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _notificationTemplateService.GetNotificationTemplateManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("NotificationTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(NotificationTemplateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _notificationTemplateService.SaveNotificationTemplate(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("Edit", new { id = model.Id });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Delete

        [HttpPost]
        public JsonResult DeleteNotificationTemplate(int id)
        {
            return Json(_notificationTemplateService.DeleteNotificationTemplate(id));
        }

        #endregion

        #region Details

        [AdministratorNavigation("Notification_Template_Details", "Notification_Templates", "Notification_Template_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _notificationTemplateService.GetNotificationTemplateDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("NotificationTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateNotificationTemplateData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_notificationTemplateService.UpdateNotificationTemplateData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #endregion
    }
}
