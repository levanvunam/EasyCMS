using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Html;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Exceptions.Sites;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Models.Users.Logins;
using EzCMS.Core.Models.Users.Settings;
using EzCMS.Core.Services.Users;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    public class AccountController : BackendController
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        #region Login

        public ActionResult Login(string returnUrl)
        {
            var model = new LoginModel
            {
                ReturnUrl = returnUrl
            };
            if (!model.Enable)
            {
                throw new EzCMSNotFoundException();
            }
            return View("Login/Login", model);
        }

        [HttpPost]
        public JsonResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _userService.Login(model);

                return Json(response);
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        [EzCMSAuthorize]
        public ActionResult LoginSuccess()
        {
            var model = new LoginSetupModel(LoginEnums.LoginTemplateConfiguration.LoginSuccess);
            if (!model.Enable)
            {
                throw new EzCMSNotFoundException();
            }
            return View("Login/LoginSuccess", model);
        }

        #endregion

        #region Change password after login

        [EzCMSAuthorize]
        public ActionResult ChangePasswordAfterLogin(string returnUrl = null)
        {
            var model = new ChangePasswordAfterLoginModel { ReturnUrl = returnUrl };
            if (!model.Enable)
            {
                throw new EzCMSNotFoundException();
            }
            return View("Login/ChangePasswordAfterLogin", model);
        }

        [EzCMSAuthorize]
        [HttpPost]
        public ActionResult ChangePasswordAfterLogin(ChangePasswordAfterLoginModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserId = WorkContext.CurrentUser.Id;

                var response = _userService.ChangePasswordAfterLogin(model);

                return Json(response);
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #endregion

        #region Forgot Password

        public ActionResult ForgotPassword()
        {
            var model = new ForgotPasswordModel();
            if (!model.Enable)
            {
                throw new EzCMSNotFoundException();
            }
            return View("ForgotPassword/ForgotPassword", model);
        }

        [HttpPost]
        public JsonResult ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _userService.ForgotPassword(model);
                return Json(result);
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        public ActionResult ForgotPasswordSuccess()
        {
            var model = new LoginSetupModel(LoginEnums.LoginTemplateConfiguration.ForgotPasswordSuccess);
            if (!model.Enable)
            {
                throw new EzCMSNotFoundException();
            }
            return View("ForgotPassword/ForgotPasswordSuccess", model);
        }

        #endregion

        #region Reset Password

        public ActionResult ResetPassword(int userId, string code)
        {
            var isValidCode = _userService.IsResetPasswordCodeValid(userId, code);
            var model = new ResetPasswordModel
            {
                IsValidCode = isValidCode,
                UserId = userId,
                SecurityCode = code
            };

            if (!model.Enable)
            {
                throw new EzCMSNotFoundException();
            }

            return View("ResetPassword/ResetPassword", model);
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _userService.ResetPassword(model);
                return Json(result);
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        public ActionResult ResetPasswordSuccess()
        {
            var model = new LoginSetupModel(LoginEnums.LoginTemplateConfiguration.ResetPasswordSuccess);
            if (!model.Enable)
            {
                throw new EzCMSNotFoundException();
            }
            return View("ResetPassword/ResetPasswordSuccess", model);
        }

        #endregion

        #region Register

        public ActionResult Register()
        {
            var model = new RegisterModel();
            if (!model.Enable)
            {
                throw new EzCMSNotFoundException();
            }
            return View("Register/Register", model);
        }

        [HttpPost]
        public JsonResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _userService.Register(model);
                return Json(response);
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        public ActionResult RegisterSuccess()
        {
            var model = new LoginSetupModel(LoginEnums.LoginTemplateConfiguration.RegisterSuccess);
            if (!model.Enable)
            {
                throw new EzCMSNotFoundException();
            }
            return View("Register/RegisterSuccess", model);
        }

        #endregion

        #region Log Out

        public ActionResult LogOut(string returnUrl)
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = System.Web.HttpContext.Current.Request.UrlReferrer != null ? System.Web.HttpContext.Current.Request.UrlReferrer.AbsolutePath : string.Empty;

            return Redirect(returnUrl);
        }

        #endregion

        #region My Profile

        [EzCMSAuthorize(IsAdministrator = true)]
        public ActionResult MyProfile()
        {
            if (WorkContext.CurrentUser == null)
            {
                throw new EzCMSUnauthorizeException();
            }

            var model = _userService.GetUserProfile(WorkContext.CurrentUser.Identity);

            if (model == null) throw new EzCMSUnauthorizeException();

            return View("MyProfiles/MyProfile", model);
        }

        [HttpPost]
        [EzCMSAuthorize(IsAdministrator = true)]
        public JsonResult UploadAvatar(HttpPostedFileBase avatar)
        {
            return Json(_userService.UploadAvatar(WorkContext.CurrentUser.Id, avatar));
        }

        #region Settings

        [HttpPost]
        [EzCMSAuthorize(IsAdministrator = true)]
        public JsonResult Settings(ManageSettingModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_userService.SaveSetting(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #endregion

        #region Change Password

        [HttpPost]
        [EzCMSAuthorize]
        public JsonResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_userService.ChangePassword(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #endregion

        #endregion

        #region User Settings

        [HttpPost]
        [EzCMSAuthorize]
        public JsonResult SaveCurrentUserGridConfig(GridSettingItem setting)
        {
            return Json(_userService.SaveUserGridSetting(WorkContext.CurrentUser.Id, setting));
        }

        [EzCMSAuthorize]
        public string GetGridSettings(string name)
        {
            var setting = _userService.GetUserGridSetting(WorkContext.CurrentUser.Id, name);
            if (setting == null)
            {
                setting = new GridSettingItem
                {
                    Name = name,
                    Columns = new List<ColumnSetting>()
                };
            }
            return SerializeUtilities.Serialize(setting).RemoveNewLine();
        }

        #endregion

        #region Manage Expiry Date

        public ActionResult ExtendAccountExpiresDate(string authorizeCode)
        {
            var response = _userService.ExtendAccountExpiratesDate(authorizeCode);
            return View("ExtendAccountExpiresDate/ExtendAccountExpiresDate", response);
        }

        #endregion
    }
}
