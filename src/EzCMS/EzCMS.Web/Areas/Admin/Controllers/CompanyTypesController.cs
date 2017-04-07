using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.CompanyTypes;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.CompanyTypes;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class CompanyTypesController : BackendController
    {
        private readonly ICompanyTypeService _companyTypeService;
        public CompanyTypesController(ICompanyTypeService companyTypeService)
        {
            _companyTypeService = companyTypeService;
        }

        [AdministratorNavigation("Company_Types", "Contact_Holder", "Company_Types", "fa-building-o", 30)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int? associateId)
        {
            return JsonConvert.SerializeObject(_companyTypeService.SearchCompanyTypes(si, associateId));
        }

        /// <summary>
        /// Export CompanyTypes
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, int? associateId)
        {
            var workbook = _companyTypeService.Exports(si, gridExportMode, associateId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "CompanyTypes.xls");
        }

        #region Create

        [AdministratorNavigation("Company_Type_Create", "Company_Types", "Company_Type_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _companyTypeService.GetCompanyTypeManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(CompanyTypeManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _companyTypeService.SaveCompanyType(model);
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

        [AdministratorNavigation("Company_Type_Edit", "Company_Types", "Company_Type_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _companyTypeService.GetCompanyTypeManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("CompanyType_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(CompanyTypeManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _companyTypeService.SaveCompanyType(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            if (!string.IsNullOrEmpty(returnUrl))
                                return Redirect(returnUrl);
                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = model.Id, returnUrl });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Popup Edit

        public ActionResult PopupEdit(int id)
        {
            SetupPopupAction();
            var model = _companyTypeService.GetCompanyTypeManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("CompanyType_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(CompanyTypeManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _companyTypeService.SaveCompanyType(model);
                SetResponseMessage(response);
                if (response.Success)
                {

                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox", new CloseFancyBoxViewModel
                            {
                                IsReload = false,
                                ReturnUrl = string.Empty
                            });
                        case SubmitType.SaveAndContinueEdit:
                            return RedirectToAction("PopupEdit", new { id = model.Id });
                    }
                }
            }
            SetupPopupAction();
            return View(model);
        }

        #endregion

        #region Details

        [AdministratorNavigation("Company_Type_Details", "Company_Types", "Company_Type_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _companyTypeService.GetCompanyTypeDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("CompanyType_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateCompanyTypeData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_companyTypeService.UpdateCompanyTypeData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteCompanyType(int id)
        {
            return Json(_companyTypeService.DeleteCompanyType(id));
        }

        #endregion

        #endregion

        [HttpPost]
        public JsonResult DeleteAssociateCompanyTypeMapping(int companyTypeId, int associateId)
        {
            return Json(_companyTypeService.DeleteAssociateCompanyTypeMapping(companyTypeId, associateId));
        }
    }
}
