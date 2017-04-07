using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.FormComponents;
using EzCMS.Core.Services.FormComponents;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class FormComponentsController : BackendController
    {
        private readonly IFormComponentService _formComponentService;
        public FormComponentsController(IFormComponentService formComponentService)
        {
            _formComponentService = formComponentService;
        }

        [AdministratorNavigation("Form_Components", "Form_Settings", "Form_Components", "fa-code", 30)]
        public ActionResult Index()
        {
            var searchModel = new FormComponentSearchModel();
            return View(searchModel);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, FormComponentSearchModel model)
        {
            return JsonConvert.SerializeObject(_formComponentService.SearchFormComponents(si, model));
        }

        /// <summary>
        /// Export form components
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, FormComponentSearchModel model)
        {
            var workbook = _formComponentService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "FormComponents.xls");
        }

        #region Create

        [AdministratorNavigation("Form_Component_Create", "Form_Components", "Form_Component_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _formComponentService.GetFormComponentManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormComponentManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _formComponentService.SaveFormComponent(model);
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

        [AdministratorNavigation("Form_Component_Edit", "Form_Components", "Form_Component_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _formComponentService.GetFormComponentManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("FormComponent_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormComponentManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _formComponentService.SaveFormComponent(model);
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

        [AdministratorNavigation("Form_Component_Details", "Form_Components", "Form_Component_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _formComponentService.GetFormComponentDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("FormComponent_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateFormComponentData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_formComponentService.UpdateFormComponentData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteFormComponent(int id)
        {
            return Json(_formComponentService.DeleteFormComponent(id));
        }

        #endregion

        #endregion
    }
}
