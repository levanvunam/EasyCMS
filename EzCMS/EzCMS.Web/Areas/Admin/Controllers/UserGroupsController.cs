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
using EzCMS.Core.Models.UserGroups;
using EzCMS.Core.Services.UserGroups;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Areas.Admin.Controllers
{
    [EzCMSAuthorize(IsAdministrator = true)]
    public class UserGroupsController : BackendController
    {
        private readonly IUserGroupService _userGroupService;
        public UserGroupsController(IUserGroupService userGroupService)
        {
            _userGroupService = userGroupService;
        }

        #region Grid

        [AdministratorNavigation("User_Groups", "User_Holder", "User_Groups", "fa-users", 20)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string _AjaxBinding(JqSearchIn si, int? toolbarId)
        {
            return JsonConvert.SerializeObject(_userGroupService.SearchUserGroups(si, toolbarId));
        }

        /// <summary>
        /// Export user groups
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="toolbarId"></param>
        /// <returns></returns>
        public ActionResult Exports(JqSearchIn si, GridExportMode gridExportMode, int? toolbarId)
        {
            var workbook = _userGroupService.Exports(si, gridExportMode, toolbarId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "UserGroups.xls");
        }

        [HttpGet]
        public string _AjaxBindingByUser(JqSearchIn si, int userId)
        {
            return JsonConvert.SerializeObject(_userGroupService.SearchUserGroupsOfUser(si, userId));
        }

        /// <summary>
        /// Export user groups
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult ExportsUserGroupsOfUser(JqSearchIn si, GridExportMode gridExportMode, int userId)
        {
            var workbook = _userGroupService.ExportsUserGroupsOfUser(si, gridExportMode, userId);

            var output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", "UserGroups.xls");
        }

        #region For related tabs

        [HttpGet]
        public string _AjaxBindingForGroupPermissions(JqSearchIn si, int userGroupId)
        {
            return JsonConvert.SerializeObject(_userGroupService.SearchGroupPermissions(si, userGroupId));
        }

        [HttpGet]
        public string _AjaxBindingForPageSecurities(JqSearchIn si, int userGroupId)
        {
            return JsonConvert.SerializeObject(_userGroupService.SearchPageSecurities(si, userGroupId));
        }

        [HttpGet]
        public string _AjaxBindingForProtectedDocuments(JqSearchIn si, int userGroupId)
        {
            return JsonConvert.SerializeObject(_userGroupService.SearchProtectedDocuments(si, userGroupId));
        }

        #endregion

        #endregion

        #region Permissions

        public ActionResult Permissions(int id)
        {
            var model = _userGroupService.GetPermissionSettings(id);
            return View(model);
        }

        [HttpPost]
        [ActionName("Permissions")]
        public ActionResult PermissionsPost(int id)
        {
            var ids = new List<int>();
            foreach (string key in Request.Form)
            {
                if (Request.Form[key].Equals("on"))
                {
                    ids.Add(int.Parse(key));
                }
            }

            return Json(_userGroupService.SavePermissions(ids, id));
        }

        #endregion

        #region Create

        [AdministratorNavigation("User_Group_Create", "User_Groups", "User_Group_Create", "fa-plus", 10, false)]
        public ActionResult Create()
        {
            var model = _userGroupService.GetUserGroupManageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(UserGroupManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _userGroupService.SaveUserGroupManageModel(model);
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

        [AdministratorNavigation("User_Group_Edit", "User_Groups", "User_Group_Edit", "fa-edit", 20, false)]
        public ActionResult Edit(int id)
        {
            var model = _userGroupService.GetUserGroupManageModel(id);
            if (!model.Id.HasValue)
            {
                SetErrorMessage(T("UserGroup_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(UserGroupManageModel model, string returnUrl, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _userGroupService.SaveUserGroupManageModel(model);
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
            var model = _userGroupService.GetUserGroupManageModel(id);
            if (model == null)
            {
                SetErrorMessage(T("UserGroup_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PopupEdit(UserGroupManageModel model, SubmitType submit)
        {
            if (ModelState.IsValid)
            {
                var response = _userGroupService.SaveUserGroupManageModel(model);
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

        [AdministratorNavigation("User_Group_Details", "User_Groups", "User_Group_Details", "fa-search-plus", 30, false)]
        public ActionResult Details(int id)
        {
            var model = _userGroupService.GetUserGroupDetailModel(id);
            if (model == null)
            {
                SetErrorMessage(T("UserGroup_Message_ObjectNotFound"));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateUserGroupData(XEditableModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_userGroupService.UpdateUserGroupData(model));
            }

            return Json(new ResponseModel
            {
                Success = false,
                Message = ModelState.BuildValidationMessages()
            });
        }

        #region Methods

        [HttpPost]
        public JsonResult DeleteUserGroup(int id)
        {
            return Json(_userGroupService.DeleteUserGroup(id));
        }

        [HttpPost]
        public JsonResult DeleteUserGroupPageSecurityMapping(int userGroupId, int pageId)
        {
            var response = _userGroupService.DeleteUserGroupPageSecurityMapping(userGroupId, pageId);
            return Json(response);
        }

        [HttpPost]
        public JsonResult DeleteUserGroupProtectedDocumentMapping(int userGroupId, int protectedDocumentId)
        {
            var response = _userGroupService.DeleteUserGroupProtectedDocumentMapping(userGroupId, protectedDocumentId);
            return Json(response);
        }

        #endregion

        #endregion
    }
}
