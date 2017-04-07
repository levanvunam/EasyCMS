using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Authorize;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Framework.Navigations;
using EzCMS.Core.Models.Shared;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Models.Users.Logins;
using EzCMS.Core.Services.Users;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class UsersController : BackendController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        #region Grid

        [AdministratorNavigation("User_Holder", "", "User_Holder", "fa-users", 80, true, true)]
        [AdministratorNavigation("Users", "User_Holder", "Users", "fa-users", 10)]
        public ActionResult Index()
        {
            var model = new UserSearchModel();
            return View(model);
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, UserSearchModel model)
        {
            return JsonConvert.SerializeObject(_userService.SearchUsers(si, model));
        }

        /// <summary>
        /// Export users
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, UserSearchModel model)
        {
            var workbook = _userService.Exports(si, gridExportMode, model);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "Users.xls");
        }

        #endregion

        #region Create

        [AdministratorNavigation("User_Create", "Users", "User_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _userService.GetUserManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(UserManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _userService.SaveUserManageModel(model);
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

        [AdministratorNavigation("User_Edit", "Users", "User_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _userService.GetUserManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("User_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(UserManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _userService.SaveUserManageModel(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    switch (submit)
                    {
                        case SubmitType.Save:
                            if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);

                            return RedirectToAction("Index");
                        default:
                            return RedirectToAction("Edit", new { id = model.Id, returnUrl });
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Popup Create

        public ActionResult PopupCreateFromContact(int contactId)
        {
            SetupPopupAction();
            var model = _userService.GetContactUserManageModel(contactId);

            if (model == null)
            {
                SetErrorMessage(T("User_Message_InvalidContactIdForCreatingUser"));
                return View("ShowMessageAndCloseFancyBox", new CloseFancyBoxViewModel
                {
                    IsReload = true
                });
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult PopupCreateFromContact(ContactUserManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _userService.SaveContactUserManageModel(model);
                SetResponseMessage(response);
                if (response.Success)
                {
                    var id = (int)response.Data;
                    switch (submit)
                    {
                        case SubmitType.PopupSave:
                            return View("CloseFancyBox",
                                new CloseFancyBoxViewModel
                                {
                                    IsReload = false
                                });
                        default:
                            return RedirectToAction("PopupEdit", new { id });
                    }
                }
            }
            SetupPopupAction();
            return View(model);
        }

        public ActionResult PopupCreate()
        {
            SetupPopupAction();
            var model = _userService.GetSimpleUserManageModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult PopupCreate(SimpleUserManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _userService.SaveSimpleUserManageModel(model);
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
                            return RedirectToAction("PopupEdit", new { id = (int)response.Data });
                    }
                }
            }
            SetupPopupAction();
            return View(model);
        }

        #endregion

        #region Popup Edit

        public ActionResult PopupEdit(int id)
        {
            SetupPopupAction();
            var model = _userService.GetSimpleUserManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("User_Message_ObjectNotFound"));
                return View("CloseFancyBox",
                    new CloseFancyBoxViewModel
                    {
                        IsReload = true
                    });
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult PopupEdit(SimpleUserManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _userService.SaveSimpleUserManageModel(model);
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
                            return RedirectToAction("PopupEdit", new { id = model.Id });
                    }
                }
            }
            SetupPopupAction();
            return View(model);
        }

        #endregion

        #region Details

        [AdministratorNavigation("User_Details", "Users", "User_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _userService.GetUserDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("User_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateUserData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_userService.UpdateUserData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteUser(int id)
        {
            return Json(_userService.DeleteUser(id));
        }

        [HttpPost]
        public JsonResult DeleteUserGroupUserMapping(int userGroupId, int userId)
        {
            var response = _userService.DeleteUserUserGroupMapping(userGroupId, userId);
            return Json(response);
        }

        #endregion

        #endregion

        [HttpPost]
        public JsonResult ForgotPassword(string username)
        {
            var model = new ForgotPasswordModel
            {
                Username = username
            };
            var response = _userService.ForgotPassword(model);

            response.SetMessage(response.Success
                ? T("User_Message_SendForgotPasswordSuccessfully")
                : T("User_Message_SendForgotPasswordFailure"));

            return Json(response);
        }
    }
}
