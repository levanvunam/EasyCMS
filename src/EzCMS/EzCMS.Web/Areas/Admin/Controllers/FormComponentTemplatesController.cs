using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.FormComponentTemplates;
using EzCMS.Core.Services.FormComponentTemplates;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class FormComponentTemplatesController : BackendController
    {
        private readonly IFormComponentTemplateService _formComponentTemplateService;
        public FormComponentTemplatesController(IFormComponentTemplateService formComponentTemplateService)
        {
            _formComponentTemplateService = formComponentTemplateService;
        }

        [AdministratorNavigation("Form_Component_Templates", "Form_Settings", "Form_Component_Templates", "fa-file-text", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_formComponentTemplateService.SearchFormComponentTemplates(si));
        }

        /// <summary>
        /// Export form component templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _formComponentTemplateService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "FormComponentTemplates.xls");
        }

        #region Create

        [AdministratorNavigation("Form_Component_Template_Create", "Form_Component_Templates", "Form_Component_Template_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _formComponentTemplateService.GetFormComponentTemplateManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormComponentTemplateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _formComponentTemplateService.SaveFormComponentTemplate(model);
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

        [AdministratorNavigation("Form_Component_Template_Edit", "Form_Component_Templates", "Form_Component_Template_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _formComponentTemplateService.GetFormComponentTemplateManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("FormComponentTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormComponentTemplateManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _formComponentTemplateService.SaveFormComponentTemplate(model);
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

        #region Details

        [AdministratorNavigation("Form_Component_Template_Details", "Form_Component_Templates", "Form_Component_Template_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _formComponentTemplateService.GetFormComponentTemplateDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("FormComponentTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateFormComponentTemplateData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_formComponentTemplateService.UpdateFormComponentTemplateData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteFormComponentTemplate(int id)
        {
            return Json(_formComponentTemplateService.DeleteFormComponentTemplate(id));
        }

        #endregion

        #endregion
    }
}
