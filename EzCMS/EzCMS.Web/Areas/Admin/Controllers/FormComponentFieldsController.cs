using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.FormComponentFields;
using EzCMS.Core.Services.FormComponentFields;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class FormComponentFieldsController : BackendController
    {
        private readonly IFormComponentFieldService _formComponentFieldService;
        public FormComponentFieldsController(IFormComponentFieldService formComponentFieldService)
        {
            _formComponentFieldService = formComponentFieldService;
        }

        [AdministratorNavigation("Form_Component_Fields", "Form_Settings", "Form_Component_Fields", "fa-file-code-o", 40)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int? formComponentId)
        {
            return JsonConvert.SerializeObject(_formComponentFieldService.SearchFormComponentFields(si, formComponentId));
        }

        /// <summary>
        /// Export form component fields
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="formComponentId"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, int? formComponentId)
        {
            var workbook = _formComponentFieldService.Exports(si, gridExportMode, formComponentId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "FormComponentFields.xls");
        }

        #region Create

        [AdministratorNavigation("Form_Component_Field_Create", "Form_Component_Fields", "Form_Component_Field_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _formComponentFieldService.GetFormComponentFieldManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormComponentFieldManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _formComponentFieldService.SaveFormComponentField(model);
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

        [AdministratorNavigation("Form_Component_Field_Edit", "Form_Component_Fields", "Form_Component_Field_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _formComponentFieldService.GetFormComponentFieldManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("FormComponentField_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormComponentFieldManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _formComponentFieldService.SaveFormComponentField(model);
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

        [AdministratorNavigation("Form_Component_Field_Details", "Form_Component_Fields", "Form_Component_Field_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _formComponentFieldService.GetFormComponentFieldDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("FormComponentField_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateFormComponentFieldData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_formComponentFieldService.UpdateFormComponentFieldData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteFormComponentField(int id)
        {
            return Json(_formComponentFieldService.DeleteFormComponentField(id));
        }

        #endregion

        #endregion
    }
}
