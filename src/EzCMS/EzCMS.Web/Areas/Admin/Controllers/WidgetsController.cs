using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.Widgets;
using EzCMS.Core.Services.Widgets.WidgetResolver;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize]
    public class WidgetsController : BackendController
    {
        private readonly IWidgetService _widgetService;
        public WidgetsController(IWidgetService widgetService)
        {
            _widgetService = widgetService;
        }

        #region Grid

        [AdministratorNavigation("Widgets", "Module_Management", "Widgets", "fa-code", 10)]
        public ActionResult Index()
        {
            var model = new WidgetSearchModel();
            return View(model);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, WidgetSearchModel model)
        {
            return JsonConvert.SerializeObject(_widgetService.SearchWidgets(si, model));
        }

        [HttpGet]
        public string _AjaxWidgetParameters(JqSearchIn si, string widget)
        {
            return JsonConvert.SerializeObject(_widgetService.SearchWidgetParameters(si, widget));
        }

        /// <summary>
        /// Export widgets
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, WidgetSearchModel model)
        {
            var workbook = _widgetService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Widgets.xls");
        }

        #endregion

        #region Methods

        [HttpPost]
        public JsonResult GeneratePropertyDropdownFromWidget(string widget)
        {
            var model = _widgetService.GetPropertyListFromWidget(widget);
            return Json(new ResponseModel
            {
                Success = true,
                Data = RenderPartialViewToString("Partials/_PropertyDropdown", model)
            });
        }

        [HttpPost]
        public JsonResult GeneratePropertyValueDropdownFromWidget(string widget)
        {
            var model = _widgetService.GetPropertyValueListFromWidget(widget);
            return Json(new ResponseModel
            {
                Success = true,
                Data = model
            });
        }

        [HttpPost]
        public JsonResult GeneratePropertyDropdownFromType(string type)
        {
            var model = _widgetService.GetPropertyListFromType(type);
            return Json(new ResponseModel
            {
                Success = true,
                Data = RenderPartialViewToString("Partials/_PropertyDropdown", model)
            });
        }

        [HttpPost]
        public JsonResult GeneratePropertyTemplate(string type, string name)
        {
            return Json(_widgetService.GeneratePropertyTemplate(type, name));
        }
        #endregion

        #region Widget builder

        public PartialViewResult WidgetDropdown(string callback)
        {
            var model = _widgetService.GetWidgetDropdownModel(callback);
            return PartialView("WidgetBuilders/WidgetDropdown", model);
        }

        #region Select Widget

        public ActionResult SelectWidgets()
        {
            var model = _widgetService.GetSelectWidgetModel();
            return View("WidgetBuilders/SelectWidgets", model);
        }

        #endregion

        #region Generate Widget

        public ActionResult GenerateWidget(string id)
        {
            var model = _widgetService.GetWidgetManageModel(id);

            if (model == null)
            {
                SetErrorMessage(T("Widget_Message_NoEditorForWidget"));
                return RedirectToAction("SelectWidgets", new
                {
                    callback = Request["callback"],
                    isCkEditor = Request["isCkEditor"]
                });
            }

            //Send callback function to view
            return View("WidgetBuilders/GenerateWidget", model);
        }

        [HttpPost]
        public ActionResult GenerateWidget(WidgetManageModel model)
        {
            var widgetInstanse =
                HostContainer.GetInstances<IWidgetResolver>()
                    .FirstOrDefault(c => c.GetSetup().Widget.Equals(model.Widget));

            if (widgetInstanse != null)
            {
                if (TryUpdateModelWithType(widgetInstanse))
                {
                    return Json(new ResponseModel
                    {
                        Success = true,
                        Data = widgetInstanse.GenerateFullWidget()
                    });
                }
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = T("Widget_Message_ObjectNotFound")
            });
        }

        #endregion

        public ActionResult IframePreview(string widget)
        {
            var model = _widgetService.GetPreviewModel(widget);

            return View("WidgetBuilders/IframePreview", model);
        }

        #endregion
    }
}
