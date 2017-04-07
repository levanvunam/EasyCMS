using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Models.Users.ExtendExpirationDate;
using EzCMS.Core.Models.Users.Logins;
using EzCMS.Core.Models.Users.Settings;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace EzCMS.Core.Services.Users
{
    [Register(Lifetime.PerInstance)]
    public interface IUserService : IBaseService<User>
    {
        /// <summary>
        /// Get list of permissions of user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<int> GetUserPermissions(int userId);

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="isRemote"></param>
        /// <returns></returns>
        User GetByUsername(string username, bool? isRemote = null);

        /// <summary>
        /// Get active user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        User GetActiveUser(string username);

        /// <summary>
        /// Get current user info
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        UserProfileModel GetUserProfile(string username);

        /// <summary>
        /// Get users
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetUsers(int? userId = null);

        #region Grid

        /// <summary>
        /// Search users
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchUsers(JqSearchIn si, UserSearchModel model);

        /// <summary>
        /// Export users
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, UserSearchModel model);

        #endregion

        #region Manage

        /// <summary>
        /// Get contact user manage model
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        ContactUserManageModel GetContactUserManageModel(int contactId);

        /// <summary>
        /// Save contact user manage model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveContactUserManageModel(ContactUserManageModel model);

        /// <summary>
        /// Get simple user manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SimpleUserManageModel GetSimpleUserManageModel(int? id = null);

        /// <summary>
        /// Save simple user manage model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveSimpleUserManageModel(SimpleUserManageModel model);

        /// <summary>
        /// Get user manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserManageModel GetUserManageModel(int? id = null);

        /// <summary>
        /// Save user manage model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="avatar"></param>
        /// <param name="contactId"></param>
        /// <returns></returns>
        ResponseModel SaveUserManageModel(UserManageModel model, HttpPostedFileBase avatar = null, int? contactId = null);

        /// <summary>
        /// Change user status
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        ResponseModel ChangeUserStatus(int userId, UserEnums.UserStatus userStatus);

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        ResponseModel Delete(User user);

        /// <summary>
        /// Delete user by User Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ResponseModel DeleteUser(int userId);

        /// <summary>
        /// Delete mapping between user group and user
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        ResponseModel DeleteUserUserGroupMapping(int userGroupId, int userId);

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        ResponseModel UpdateUser(User user);

        /// <summary>
        /// Upload avatar
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="avatar"></param>
        /// <returns></returns>
        ResponseModel UploadAvatar(int userId, HttpPostedFileBase avatar);

        #endregion

        #region Login / Register / Change Password

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel ChangePassword(ChangePasswordModel model);

        /// <summary>
        /// Change password after login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel ChangePasswordAfterLogin(ChangePasswordAfterLoginModel model);

        /// <summary>
        /// Check user login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        User CheckLogin(string username, string password, out dynamic session);

        /// <summary>
        /// Check user login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        User CheckLogin(string username, string password);

        /// <summary>
        /// Login function
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel Login(LoginModel model);

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel ForgotPassword(ForgotPasswordModel model);

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel ResetPassword(ResetPasswordModel model);

        #region Register

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel Register(RegisterModel model);

        #endregion

        #endregion

        #region Validation

        /// <summary>
        /// Check if user exists
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        bool IsUsernameExisted(int? userId, string username);

        /// <summary>
        /// Check if reset password code is valid
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        bool IsResetPasswordCodeValid(int userId, string code);

        #endregion

        #region Permissions

        /// <summary>
        /// Check if user has one of permissions
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        bool HasOneOfPermissions(int userId, params Permission[] permissions);

        /// <summary>
        /// Check if user has all permissions
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        bool HasPermissions(int userId, params Permission[] permissions);

        #endregion

        #region Company Admin

        /// <summary>
        /// Search company users
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchCompanyUsers(JqSearchIn si, UserSearchModel model);

        /// <summary>
        /// Export company users
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook ExportCompanyUsers(JqSearchIn si, GridExportMode gridExportMode, UserSearchModel model);

        /// <summary>
        /// Get user details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserDetailModel GetCompanyUserDetailModel(int id);

        /// <summary>
        /// Change user status
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        ResponseModel ChangeCompanyUserStatus(int userId, UserEnums.UserStatus userStatus);

        #endregion

        #region User settings

        /// <summary>
        /// Get current user grid setting
        /// </summary>
        /// <returns></returns>
        GridSettingItem GetUserGridSetting(int userId, string name);

        /// <summary>
        /// Save user grid setting
        /// </summary>
        /// <returns></returns>
        ResponseModel SaveUserGridSetting(int userId, GridSettingItem setting);

        /// <summary>
        /// Save setting
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveSetting(ManageSettingModel model);

        #endregion

        #region Details

        /// <summary>
        /// Get user details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserDetailModel GetUserDetailModel(int id);

        /// <summary>
        /// Update user data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateUserData(XEditableModel model);

        #endregion

        #region Manage Account Expires

        /// <summary>
        /// Get a list of users that near expiration date
        /// </summary>
        /// <returns></returns>
        IEnumerable<User> GetUsersNearExpirationDate();

        /// <summary>
        /// Get a list of users that need to be deactivated
        /// </summary>
        /// <returns></returns>
        IEnumerable<User> GetUsersNeedDeactive();

        /// <summary>
        /// Deactive expired account
        /// </summary>
        /// <param name="user"></param>
        ResponseModel DeactiveExpiredAccount(User user);

        /// <summary>
        /// Send deactivation email to user
        /// </summary>
        /// <param name="user"></param>
        void SendDeactivationEmail(User user);

        /// <summary>
        /// Extend account expirates date
        /// </summary>
        /// <param name="authorizeCode"></param>
        /// <returns></returns>
        ExtendExpirationDateResponseModel ExtendAccountExpiratesDate(string authorizeCode);

        #endregion
    }
}