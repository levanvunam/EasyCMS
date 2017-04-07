using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.EmailLogs;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.EmailAccounts;
using EzCMS.Core.Services.EmailLogs;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class EmailLogsController : BackendController
    {
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailAccountService _emailAccountService;
        public EmailLogsController(IEmailLogService emailLogService, IEmailAccountService emailAccountService)
        {
            _emailLogService = emailLogService;
            _emailAccountService = emailAccountService;
        }

        #region Grid

        [AdministratorNavigation("Email_Logs", "Email_Holder", "Email_Logs", "fa-envelope-o", 30)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int? emailAccountId)
        {
            return JsonConvert.SerializeObject(_emailLogService.SearchEmailLogs(si));
        }

        /// <summary>
        /// Export email queues
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _emailLogService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "EmailLogs.xls");
        }

        [HttpGet]
        public string _AjaxBindingForLogs(JqSearchIn si, int emailLogId)
        {
            return JsonConvert.SerializeObject(_emailLogService.SearchLogs(si, emailLogId));
        }

        #endregion

        #region Details
        [AdministratorNavigation("Email_Log_Details", "Email_Logs", "Email_Log_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _emailLogService.GetEmailLogDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("EmailLog_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult ResendEmail(int id)
        {
            var model = _emailLogService.GetResendEmailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("EmailLog_Message_ObjectNotFound"));
                return View("CloseFancyBox",
                    new CloseFancyBoxViewModel
                    {
                        IsReload = true
                    });
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ResendEmail(ResendEmailModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _emailLogService.ResendEmail(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    return View("CloseFancyBox",
                        new CloseFancyBoxViewModel
                        {
                            IsReload = true
                        });
                }
            }
            model.EmailAccounts = _emailAccountService.GetEmailAccounts();
            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteEmail(int id)
        {
            return Json(_emailLogService.DeleteEmail(id));
        }

        #endregion
    }
}