using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.FormTabs;
using EzCMS.Core.Services.FormTabs;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class FormTabsController : BackendController
    {
        private readonly IFormTabService _formTabService;

        public FormTabsController(IFormTabService formTabService)
        {
            _formTabService = formTabService;
        }

        [AdministratorNavigation("Form_Tabs", "Form_Settings", "Form_Tabs", "fa-list-ul", 10)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_formTabService.SearchFormTabs(si));
        }

        /// <summary>
        /// Export FormTabs
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _formTabService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "FormTabs.xls");
        }

        #region Create		
		
		[AdministratorNavigation("Form_Tab_Create", "Form_Tabs", "Form_Tab_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _formTabService.GetFormTabManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormTabManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _formTabService.SaveFormTab(model);
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

        [AdministratorNavigation("Form_Tab_Edit", "Form_Tabs", "Form_Tab_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _formTabService.GetFormTabManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("FormTab_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormTabManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _formTabService.SaveFormTab(model);
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

        [AdministratorNavigation("Form_Tab_Details", "Form_Tabs", "Form_Tab_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _formTabService.GetFormTabDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("FormTab_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateFormTabData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_formTabService.UpdateFormTabData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteFormTab(int id)
        {
            return Json(_formTabService.DeleteFormTab(id));
        }

        #endregion

        #endregion
    }
}
