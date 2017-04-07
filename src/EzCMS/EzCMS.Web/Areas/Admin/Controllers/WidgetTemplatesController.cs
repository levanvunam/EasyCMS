using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.WidgetTemplates;
using EzCMS.Core.Services.WidgetTemplates;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class WidgetTemplatesController : BackendController
    {
        private readonly IWidgetTemplateService _widgetTemplateService;
        public WidgetTemplatesController(IWidgetTemplateService widgetTemplateService)
        {
            _widgetTemplateService = widgetTemplateService;
        }

        #region Grid

        [AdministratorNavigation("Widget_Templates", "Module_Management", "Widget_Templates", "fa-file-text", 20)]
        public ActionResult Index()
        {
            var searchModel = new WidgetTemplateSearchModel();
            return View(searchModel);
        }

        #region Ajax Methods

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, WidgetTemplateSearchModel model)
        {
            return JsonConvert.SerializeObject(_widgetTemplateService.SearchTemplates(si, model));
        }

        #endregion

        /// <summary>
        /// Export templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, WidgetTemplateSearchModel model)
        {
            var workbook = _widgetTemplateService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Templates.xls");
        }

        #endregion

        #region Create

        [AdministratorNavigation("Widget_Template_Create", "Widget_Templates", "Widget_Template_Create", "fa-plus", 20, false)]
        public ActionResult Create(string widget, int? templateId)
        {
            var template = _widgetTemplateService.GetTemplateManageModel(widget, templateId);
            if (template == null)
            {
                SetErrorMessage(T("WidgetTemplate_Message_WidgetIsNotExisted"));
                return RedirectToAction("Index");
            }
            return View(template);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(WidgetTemplateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _widgetTemplateService.SaveTemplateManageModel(model);
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

        public ActionResult EditTemplateByName(string templateName)
        {
            var template = _widgetTemplateService.GetTemplate(templateName);
            if (template == null)
            {
                SetErrorMessage(T("WidgetTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return RedirectToAction("Edit", new { id = template.Id });
        }

        [AdministratorNavigation("Widget_Template_Edit", "Widget_Templates", "Widget_Template_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int? id, int? logId)
        {
            WidgetTemplateManageModel model = null;
            if (id.HasValue)
            {
                model = _widgetTemplateService.GetTemplateManageModel(id.Value);
            }
            else if (logId.HasValue)
            {
                model = _widgetTemplateService.GetTemplateManageModelByLogId(logId.Value);
            }
            if (model == null || model.Id == null)
            {
                SetErrorMessage(T("WidgetTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(WidgetTemplateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _widgetTemplateService.SaveTemplateManageModel(model);
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

        [HttpPost]
        public JsonResult DeleteTemplate(int id)
        {
            return Json(_widgetTemplateService.DeleteTemplate(id));
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult LoadFullContent(WidgetTemplateManageModel widgetTemplate)
        {
            var content = _widgetTemplateService.GetFullTemplate(widgetTemplate);

            return Json(new ResponseModel
            {
                Success = true,
                Data = content
            });
        }

        #region Logs

        public ActionResult Logs(int id)
        {
            var model = _widgetTemplateService.GetLogs(id);
            if (model == null)
            {
                SetErrorMessage(T("WidgetTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult GetLogs(int id, int total, int index)
        {
            var model = _widgetTemplateService.GetLogs(id, total, index);
            var content = RenderPartialViewToString("Partials/_GetLogs", model);
            var response = new ResponseModel
            {
                Success = true,
                Data = new
                {
                    model.LoadComplete,
                    model.Total,
                    content
                }
            };
            return Json(response);
        }

        #endregion
    }
}
