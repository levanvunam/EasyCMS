using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.SubscriptionTemplates;
using EzCMS.Core.Services.SubscriptionTemplates;
using EzCMS.Core.Services.Widgets;
using EzCMS.Entity.Core.Enums;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class SubscriptionTemplatesController : BackendController
    {
        private readonly ISubscriptionTemplateService _subscriptionTemplateService;
        private readonly IWidgetService _widgetService;
        public SubscriptionTemplatesController(ISubscriptionTemplateService subscriptionTemplateService, IWidgetService widgetService)
        {
            _subscriptionTemplateService = subscriptionTemplateService;
            _widgetService = widgetService;
        }

        [AdministratorNavigation("Subscriptions_Templates", "Subscription_Holder", "Subscriptions_Templates", "fa-building-o", 30)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_subscriptionTemplateService.SearchSubscriptionTemplates(si));
        }

        /// <summary>
        /// Export subscription templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _subscriptionTemplateService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "SubscriptionTemplates.xls");
        }

        /// <summary>
        /// Generate Property Dropdown from NotificationModule
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GeneratePropertyDropdown(SubscriptionEnums.SubscriptionModule module)
        {
            var type = _subscriptionTemplateService.GetNotificationEmailModelAssemblyName(module);

            var model = _widgetService.GetPropertyListFromType(type);
            return Json(new ResponseModel
            {
                Success = true,
                Data = RenderPartialViewToString("Partials/_PropertyDropdown", model)
            });
        }

        #region Edit

        [AdministratorNavigation("Subscription_Template_Edit", "Subscription_Templates", "Subscription_Template_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _subscriptionTemplateService.GetSubscriptionTemplateManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("SubscriptionTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(SubscriptionTemplateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _subscriptionTemplateService.SaveSubscriptionTemplate(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = model.Id });
                    }
                }
            }
            return View(model);
        }

        #endregion
    }
}
