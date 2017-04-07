using Ez.Framework.Core.Attributes.ActionFilters;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.Exceptions.Common;
using Ez.Framework.Core.Mvc.Helpers;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.ProtectedDocuments;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Services.ProtectedDocuments;
using System;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    public class ProtectedDocumentsController : BackendController
    {
        private readonly IDocumentService _documentService;
        public ProtectedDocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        #region Index

        [EzCMSAuthorize(IsAdministrator = true)]
        [AdministratorNavigation("Protected_Documents", "Module_Holder", "Protected_Documents", "fa-file-text-o", 130)]
        public ActionResult Index()
        {
            var model = _documentService.GetProtectedDocumentWidget(EzCMSContants.ProtectedDocumentPath);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetDocuments(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = EzCMSContants.ProtectedDocumentPath.GetUniqueLink();
                }

                var documents = _documentService.GetDocuments(path);

                return Json(new ResponseModel
                {
                    Success = true,
                    Data = documents
                });
            }
            catch (Exception exception)
            {
                var response = new ResponseModel
                {
                    Success = true,
                    Message = exception.Message
                };
                return Json(response);
            }
        }

        [HttpPost]
        public JsonResult SearchDocuments(string path, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Json(new ResponseModel
                {
                    Success = false,
                    Message = T("ProtectedDocument_Message_EmptySearchKey")
                });
            }

            return Json(new ResponseModel
            {
                Success = true,
                Data = _documentService.SearchDocuments(path, keyword)
            });
        }

        #endregion

        #region Permission

        [EzCMSAuthorize(Permissions = new[] { Permission.ManageProtectedDocuments })]
        public ActionResult SetPermissions(string path)
        {
            SetupPopupAction();
            try
            {
                var model = _documentService.GetDocumentPermissionManageModel(path);
                return View(model);
            }
            catch (InvalidUniqueLinkException)
            {
                SetErrorMessage(T("ProtectedDocument_Message_SessionExpired"));
                return View("ShowMessageAndCloseFancyBox", new CloseFancyBoxViewModel
                {
                    IsReload = true
                });
            }
        }

        [HttpPost]
        [EzCMSAuthorize(Permissions = new[] { Permission.ManageProtectedDocuments })]
        public ActionResult SetPermissions(DocumentPermissionManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _documentService.SaveDocumentPermissionManageModel(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox",
                                new CloseFancyBoxViewModel
                                {
                                    IsReload = false
                                });
                        default:
                            return RedirectToAction("SetPermissions", new { path = model.EncryptedPath });
                    }
                }
            }
            SetupPopupAction();
            return View(model);
        }

        #endregion

        #region Feedback

        [EzCMSAuthorize]
        public ActionResult Feedback(string path)
        {
            SetupPopupAction();
            var model = _documentService.GetDocumentFeedback(path);
            return View(model);
        }

        [HttpPost]
        [EzCMSAuthorize]
        [CaptchaValidator("Captcha")]
        public ActionResult Feedback(DocumentFeedbackModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _documentService.SaveCustomerFeedback(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox",
                                new CloseFancyBoxViewModel
                                {
                                    IsReload = false
                                });
                        default:
                            return RedirectToAction("Feedback", new { path = model.EncryptedPath });
                    }
                }
            }
            SetupPopupAction();
            return View(model);
        }

        #endregion
    }
}