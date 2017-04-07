using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.FormDefaultComponents;
using EzCMS.Core.Services.FormDefaultComponents;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class FormDefaultComponentsController : BackendController
    {
        private readonly IFormDefaultComponentService _formDefaultComponentService;
        public FormDefaultComponentsController(IFormDefaultComponentService formDefaultComponentService)
        {
            _formDefaultComponentService = formDefaultComponentService;
        }

        [AdministratorNavigation("Form_Default_Components", "Form_Settings", "Form_Default_Components", "fa-check", 50)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int? formComponentTemplateId)
        {
            return JsonConvert.SerializeObject(_formDefaultComponentService.SearchFormDefaultComponents(si, formComponentTemplateId));
        }

        /// <summary>
        /// Export form default components
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="formComponentTemplateId"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, int? formComponentTemplateId)
        {
            var workbook = _formDefaultComponentService.Exports(si, gridExportMode, formComponentTemplateId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "FormDefaultComponents.xls");
        }

        #region Create

        [AdministratorNavigation("Form_Default_Component_Create", "Form_Default_Components", "Form_Default_Component_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _formDefaultComponentService.GetFormDefaultComponentManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormDefaultComponentManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _formDefaultComponentService.SaveFormDefaultComponent(model);
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

        [AdministratorNavigation("Form_Default_Component_Edit", "Form_Default_Components", "Form_Default_Component_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _formDefaultComponentService.GetFormDefaultComponentManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("FormDefaultComponent_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormDefaultComponentManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _formDefaultComponentService.SaveFormDefaultComponent(model);
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

        [AdministratorNavigation("Form_Default_Component_Details", "Form_Default_Components", "Form_Default_Component_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _formDefaultComponentService.GetFormDefaultComponentDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("FormDefaultComponent_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateFormDefaultComponentData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_formDefaultComponentService.UpdateFormDefaultComponentData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteFormDefaultComponent(int id)
        {
            return Json(_formDefaultComponentService.DeleteFormDefaultComponent(id));
        }

        #endregion

        #endregion
    }
}
