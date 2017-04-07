using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Styles;
using EzCMS.Core.Services.Styles;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class StylesController : BackendController
    {
        private readonly IStyleService _styleService;
        public StylesController(IStyleService styleService)
        {
            _styleService = styleService;
        }

        #region Grid

        [AdministratorNavigation("Styles", "Content_Settings", "Styles", "fa-file-text", 10)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Export style
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _styleService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Styles.xls");
        }

        #region Ajax Methods

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_styleService.SearchStyles(si));
        }

        #endregion

        #endregion

        #region Create

        [AdministratorNavigation("Style_Create", "Styles", "Style_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _styleService.GetStyleManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(StyleManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _styleService.SaveStyleManageModel(model);
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

        [AdministratorNavigation("Style_Edit", "Styles", "Style_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int? id, int? logId)
        {
            StyleManageModel model = null;
            if (id.HasValue)
            {
                model = _styleService.GetStyleManageModel(id.Value);
            }
            else if (logId.HasValue)
            {
                model = _styleService.GetStyleManageModelByLogId(logId.Value);
            }
            if (model == null)
            {
                SetErrorMessage(T("Style_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(StyleManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _styleService.SaveStyleManageModel(model);
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

        #region Delete

        [HttpPost]
        public JsonResult DeleteStyle(int id)
        {
            return Json(_styleService.DeleteStyle(id));
        }

        #endregion

        #region Logs

        public ActionResult Logs(int id)
        {
            var model = _styleService.GetLogs(id);
            if (model == null)
            {
                SetErrorMessage(T("Style_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult GetLogs(int id, int total, int index)
        {
            var model = _styleService.GetLogs(id, total, index);
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
