using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.EmailTemplates;
using EzCMS.Core.Services.EmailTemplates;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class EmailTemplatesController : BackendController
    {
        private readonly IEmailTemplateService _emailTemplateService;
        public EmailTemplatesController(IEmailTemplateService emailTemplateService)
        {
            _emailTemplateService = emailTemplateService;
        }

        [AdministratorNavigation("Email_Templates", "Email_Holder", "Email_Templates", "fa-file-text", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si)
        {
            return JsonConvert.SerializeObject(_emailTemplateService.SearchEmailTemplates(si));
        }

        /// <summary>
        /// Export email templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var workbook = _emailTemplateService.Exports(si, gridExportMode);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "EmailTemplates.xls");
        }

        #region Edit

        [AdministratorNavigation("Email_Template_Edit", "Email_Templates", "Email_Template_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id, string returnUrl)
        {
            var model = _emailTemplateService.GetEmailTemplateManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("EmailTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(EmailTemplateManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _emailTemplateService.SaveEmailTemplate(model);
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

        [AdministratorNavigation("Email_Template_Details", "Email_Templates", "Email_Template_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _emailTemplateService.GetEmailTemplateDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("EmailTemplate_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateEmailTemplateData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_emailTemplateService.UpdateEmailTemplateData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteEmailTemplate(int id)
        {
            return Json(_emailTemplateService.DeleteEmailTemplate(id));
        }

        #endregion

        #endregion

        #region Send Test Email Template

        public ActionResult ConfirmSendTestEmail(int? id)
        {
            var model = _emailTemplateService.GetEmailTemplateSendTestModel(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult ConfirmSendTestEmail(EmailTemplateSendTestModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_emailTemplateService.SendTestEmailTemplate(model));
            }
            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #endregion

    }
}
