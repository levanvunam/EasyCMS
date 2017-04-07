using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Models.UserGroups;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.UserGroups
{
    [Register(Lifetime.PerInstance)]
    public interface IUserGroupService : IBaseService<UserGroup>
    {
        #region Validation

        /// <summary>
        /// Check if user group name exists
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsNameExisted(int? id, string name);

        #endregion

        /// <summary>
        /// Get user groups
        /// </summary>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetUserGroups(List<int> groupIds = null);

        /// <summary>
        /// Get user group by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        UserGroup GetByName(string name);

        /// <summary>
        /// Remove toolbar from user group
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        ResponseModel RemoveToolbarFromUserGroup(int userGroupId);

        #region Grid

        /// <summary>
        /// Search user groups by toolbar id
        /// </summary>
        /// <param name="si"></param>
        /// <param name="toolbarId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchUserGroups(JqSearchIn si, int? toolbarId);

        /// <summary>
        /// Export user groups
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="toolbarId"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? toolbarId);

        /// <summary>
        /// Search user groups by user id
        /// </summary>
        /// <param name="si"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchUserGroupsOfUser(JqSearchIn si, int userId);

        /// <summary>
        /// Export user groups
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        HSSFWorkbook ExportsUserGroupsOfUser(JqSearchIn si, GridExportMode gridExportMode, int userId);

        #endregion

        #region Manage

        /// <summary>
        /// Get user group manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserGroupManageModel GetUserGroupManageModel(int? id = null);

        /// <summary>
        /// Save user group manage model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveUserGroupManageModel(UserGroupManageModel model);

        /// <summary>
        /// Delete user group
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        ResponseModel DeleteUserGroup(int userGroupId);

        /// <summary>
        /// Insert user group
        /// </summary>
        /// <param name="userGroup"></param>
        /// <returns></returns>
        ResponseModel Insert(UserGroup userGroup);

        /// <summary>
        /// Insert user groups
        /// </summary>
        /// <param name="userGroups"></param>
        /// <returns></returns>
        ResponseModel Insert(IEnumerable<UserGroup> userGroups);

        #endregion

        #region Permissions

        /// <summary>
        /// Get permission setting
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        GroupPermissionsModel GetPermissionSettings(int id);

        /// <summary>
        /// Save group permission
        /// </summary>
        /// <param name="permissionIds"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        ResponseModel SavePermissions(List<int> permissionIds, int userGroupId);

        /// <summary>
        /// Get group permissions
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<int> GetGroupPermissions(int id);

        /// <summary>
        /// Check if group has all of permissions
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        bool HasPermissions(int groupId, params Permission[] permissions);

        /// <summary>
        /// Check if group has one of permissions
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        bool HasOneOfPermissions(int groupId, params Permission[] permissions);

        #endregion

        #region Details

        /// <summary>
        /// Get user group details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserGroupDetailModel GetUserGroupDetailModel(int id);

        /// <summary>
        /// Update user group data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateUserGroupData(XEditableModel model);

        #region Methods

        /// <summary>
        /// Search protected documents
        /// </summary>
        /// <param name="si"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchProtectedDocuments(JqSearchIn si, int userGroupId);

        /// <summary>
        /// Search group permission
        /// </summary>
        /// <param name="si"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchGroupPermissions(JqSearchIn si, int userGroupId);

        /// <summary>
        /// Search page securities
        /// </summary>
        /// <param name="si"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchPageSecurities(JqSearchIn si, int userGroupId);

        /// <summary>
        /// Delete mapping between user group and protected document
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="protectedDocumentId"></param>
        /// <returns></returns>
        ResponseModel DeleteUserGroupProtectedDocumentMapping(int userGroupId, int protectedDocumentId);

        /// <summary>
        /// Delete mapping between user group and page security
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        ResponseModel DeleteUserGroupPageSecurityMapping(int userGroupId, int pageId);

        #endregion

        #endregion
    }
}