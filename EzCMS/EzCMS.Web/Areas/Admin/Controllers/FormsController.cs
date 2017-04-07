using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Forms.Setup;
using EzCMS.Core.Services.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class FormsController : BackendController
    {
        private readonly IFormService _formService;
        public FormsController(IFormService formService)
        {
            _formService = formService;
        }

        #region Grid

        [AdministratorNavigation("Form_Holder", "Module_Holder", "Form_Holder", "fa-square-o", 40, true, true)]
        [AdministratorNavigation("Form_Settings", "Form_Holder", "Form_Settings", "fa-cogs", 20, true, true)]
        [AdministratorNavigation("Forms", "Form_Holder", "Forms", "fa-behance-square", 10)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_formService.SearchForms(si));
        }

        /// <summary>
        /// Export forms
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _formService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Forms.xls");
        }

        #endregion

        #region Setup

        #region Build Form

        public ActionResult BuildForm(int? id)
        {
            var model = _formService.GetBuildFormSetupModel(id);
            return View("Setup/BuildForm", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult BuildForm(BuildFormSetupModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _formService.SaveFormSetupModel(model);

                if (response.Success)
                {
                    var id = model.Id ?? (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("Configure", new { id = id });
                        default:
                            return RedirectToAction("BuildForm", new { id = id });
                    }
                }
            }

            return View("Setup/BuildForm", model);
        }

        #region Methods

        [HttpPost]
        public JsonResult GetFormConfigurations(int? id)
        {
            return Json(new ResponseModel
            {
                Success = true,
                Data = _formService.GetConfigurations(id)
            });
        }

        #endregion

        #endregion

        #region Config Form

        [AdministratorNavigation("Form_Config", "Forms", "Form_Config", "fa-cog", 10, false)]
        public ActionResult Configure(int id)
        {
            var model = _formService.GetConfigurationSetupModel(id);

            if (model == null)
            {
                SetErrorMessage(T("Form_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View("Setup/Configure", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [AdministratorNavigation("Form_Config", "Forms", "Form_Config", "fa-cog", 10, false)]
        public ActionResult Configure(ConfigurationSetupModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _formService.SaveConfiguration(model);

                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            return RedirectToAction("EmbeddedScript", new { id = model.Id });
                        default:
                            return RedirectToAction("Configure", new { id = model.Id });
                    }
                }
            }

            return View("Setup/Configure", model);
        }

        #endregion

        #region Embedded script

        public ActionResult EmbeddedScript(int id)
        {
            var model = _formService.GetEmbeddedScriptModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Form_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View("Setup/EmbeddedScript", model);
        }

        #endregion

        #region Preview

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveDraft(int? id, string content)
        {
            return Json(_formService.SaveDraft(id, content));
        }

        public ActionResult Preview(int? id, string token)
        {
            var model = _formService.LoadPreview(id, token);
            if (model == null)
            {
                SetErrorMessage(T("Form_Message_InvalidParameters"));
                return RedirectToAction("Index");
            }
            return View("Setup/Preview", model);
        }

        #endregion

        #endregion

        [HttpPost]
        public JsonResult DeleteForm(int id)
        {
            return Json(_formService.DeleteForm(id));
        }
    }
}
