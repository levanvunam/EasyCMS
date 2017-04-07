using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Companies;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.Companies;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class CompaniesController : BackendController
    {
        private readonly ICompanyService _companyService;
        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [AdministratorNavigation("Companies", "Contact_Holder", "Companies", "fa-building", 20)]
        public ActionResult Index()
        {
            var model = new CompanySearchModel();
            return View(model);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, CompanySearchModel model)
        {
            return JsonConvert.SerializeObject(_companyService.SearchCompanies(si, model));
        }

        /// <summary>
        /// Export companies
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, CompanySearchModel model)
        {
            var workbook = _companyService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Companies.xls");
        }

        #region Create

        [AdministratorNavigation("Company_Create", "Companies", "Company_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _companyService.GetCompanyManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(CompanyManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _companyService.SaveCompany(model);
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

        [AdministratorNavigation("Company_Edit", "Companies", "Company_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _companyService.GetCompanyManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("Company_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(CompanyManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _companyService.SaveCompany(model);
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
            var model = _companyService.GetCompanyManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Company_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(CompanyManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _companyService.SaveCompany(model);
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

        [AdministratorNavigation("Company_Details", "Companies", "Company_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _companyService.GetCompanyDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("Company_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateCompanyData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_companyService.UpdateCompanyData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteCompany(int id)
        {
            return Json(_companyService.DeleteCompany(id));
        }

        #endregion

        #endregion

        [HttpPost]
        public JsonResult SearchCompanies(string keyword)
        {
            return Json(new ResponseModel
            {
                Success = true,
                Data = _companyService.SearchCompaniesByKeyword(keyword)
            });
        }

        [HttpPost]
        public JsonResult GetSelectCompanies(string companyName)
        {
            var company = _companyService.GetByName(companyName);
            if (company == null)
            {
                return Json(new ResponseModel
                {
                    Success = false,
                });
            }
            return Json(new ResponseModel
            {
                Success = true,
                Data = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = company.Name,
                        Value = company.Name
                    }
                }
            });
        }
    }
}