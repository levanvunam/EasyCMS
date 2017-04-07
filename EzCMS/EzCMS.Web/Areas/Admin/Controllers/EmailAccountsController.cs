using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.EmailAccounts;
using EzCMS.Core.Services.EmailAccounts;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class EmailAccountsController : BackendController
    {
        private readonly IEmailAccountService _emailAccountService;
        public EmailAccountsController(IEmailAccountService emailAccountService)
        {
            _emailAccountService = emailAccountService;
        }


        [AdministratorNavigation("Email_Holder", "Setting_Holder", "Email_Holder", "fa-envelope", 30)]
        [AdministratorNavigation("Email_Accounts", "Email_Holder", "Email_Accounts", "fa-envelope", 10)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_emailAccountService.SearchEmailAccounts(si));
        }

        [HttpPost]

        public JsonResult MarkAsDefault(int id)
        {
            return Json(_emailAccountService.MarkAsDefault(id));
        }

        [HttpPost]
        public JsonResult SendTestEmail(TestEmailModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_emailAccountService.SendTestEmail(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        /// <summary>
        /// Export email accounts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _emailAccountService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "EmailAccounts.xls");
        }

        #region Create

        [AdministratorNavigation("Email_Account_Create", "Email_Accounts", "Email_Account_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _emailAccountService.GetEmailAccountManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(EmailAccountManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _emailAccountService.SaveEmailAccount(model);
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

        [AdministratorNavigation("Email_Account_Edit", "Email_Accounts", "Email_Account_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _emailAccountService.GetEmailAccountManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("EmailAccount_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(EmailAccountManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _emailAccountService.SaveEmailAccount(model);
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

        #region Details

        [AdministratorNavigation("Email_Account_Details", "Email_Accounts", "Email_Account_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _emailAccountService.GetEmailAccountDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("EmailAccount_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateEmailAccountData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_emailAccountService.UpdateEmailAccountData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteEmailAccount(int id)
        {
            return Json(_emailAccountService.DeleteEmailAccount(id));
        }

        #endregion

        #endregion
    }
}
