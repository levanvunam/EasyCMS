using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Scripts;
using EzCMS.Core.Services.Scripts;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class ScriptsController : BackendController
    {
        private readonly IScriptService _scriptService;
        public ScriptsController(IScriptService scriptService)
        {
            _scriptService = scriptService;
        }

        #region Grid

        [AdministratorNavigation("Scripts", "Content_Settings", "Scripts", "fa-file-text-o", 20)]
        public ActionResult Index()
        {
            return View();
        }

        #region Ajax Methods

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_scriptService.SearchScripts(si));
        }

        #endregion

        /// <summary>
        /// Export scripts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _scriptService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Scripts.xls");
        }

        #endregion

        #region Create

        [AdministratorNavigation("Script_Create", "Scripts", "Script_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var script = _scriptService.GetScriptManageModel();
            return View(script);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ScriptManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _scriptService.SaveScriptManageModel(model);
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

        [AdministratorNavigation("Script_Edit", "Scripts", "Script_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int? id, int? logId)
        {
            ScriptManageModel model = null;
            if (id.HasValue)
            {
                model = _scriptService.GetScriptManageModel(id.Value);
            }
            else if (logId.HasValue)
            {
                model = _scriptService.GetScriptManageModelByLogId(logId.Value);
            }
            if (model == null)
            {
                SetErrorMessage(T("Script_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ScriptManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _scriptService.SaveScriptManageModel(model);
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
        public JsonResult DeleteScript(int id)
        {
            return Json(_scriptService.DeleteScript(id));
        }

        #region Logs

        public ActionResult Logs(int id)
        {
            var model = _scriptService.GetLogs(id);
            if (model == null)
            {
                SetErrorMessage(T("Script_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult GetLogs(int id, int total, int index)
        {
            var model = _scriptService.GetLogs(id, total, index);
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
