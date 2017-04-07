using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.FormDefaultComponentFields;
using EzCMS.Core.Services.FormDefaultComponentFields;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class FormDefaultComponentFieldsController : BackendController
    {
        private readonly IFormDefaultComponentFieldService _formDefaultComponentFieldService;
        public FormDefaultComponentFieldsController(IFormDefaultComponentFieldService formDefaultComponentFieldService)
        {
            _formDefaultComponentFieldService = formDefaultComponentFieldService;
        }

        [AdministratorNavigation("Form_Default_Component_Fields", "Form_Settings", "Form_Default_Component_Fields", "fa-file-code-o", 50)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int? formDefaultComponentId)
        {
            return JsonConvert.SerializeObject(_formDefaultComponentFieldService.SearchFormDefaultComponentFields(si, formDefaultComponentId));
        }

        /// <summary>
        /// Export form default component fields
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="formDefaultComponentId"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, int? formDefaultComponentId)
        {
            var workbook = _formDefaultComponentFieldService.Exports(si, gridExportMode, formDefaultComponentId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "FormDefaultComponentFields.xls");
        }

        #region Create

        [AdministratorNavigation("Form_Default_Component_Field_Create", "Form_Default_Component_Fields", "Form_Default_Component_Field_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _formDefaultComponentFieldService.GetFormDefaultComponentFieldManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormDefaultComponentFieldManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _formDefaultComponentFieldService.SaveFormDefaultComponentField(model);
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

        [AdministratorNavigation("Form_Default_Component_Field_Edit", "Form_Default_Component_Fields", "Form_Default_Component_Field_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _formDefaultComponentFieldService.GetFormDefaultComponentFieldManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("FormDefaultComponentField_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormDefaultComponentFieldManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _formDefaultComponentFieldService.SaveFormDefaultComponentField(model);
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

        [AdministratorNavigation("Form_Default_Component_Field_Details", "Form_Default_Component_Fields", "Form_Default_Component_Field_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _formDefaultComponentFieldService.GetFormDefaultComponentFieldDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("FormDefaultComponentField_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateFormDefaultComponentFieldData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_formDefaultComponentFieldService.UpdateFormDefaultComponentFieldData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteFormDefaultComponentField(int id)
        {
            return Json(_formDefaultComponentFieldService.DeleteFormDefaultComponentField(id));
        }

        #endregion

        #endregion
    }
}
