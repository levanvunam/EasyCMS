using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Models.Pages;
using EzCMS.Core.Models.ProtectedDocuments;
using EzCMS.Core.Models.UserGroups;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.GroupPermissions;
using EzCMS.Entity.Repositories.PageSecurities;
using EzCMS.Entity.Repositories.ProtectedDocumentGroups;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.UserGroups
{
    public class UserGroupService : ServiceHelper, IUserGroupService
    {
        private readonly IGroupPermissionRepository _groupPermissionRepository;
        private readonly IPageSecurityRepository _pageSecurityRepository;
        private readonly IProtectedDocumentGroupRepository _protectedDocumentGroupRepository;
        private readonly IRepository<UserGroup> _userGroupRepository;

        public UserGroupService(IGroupPermissionRepository groupPermissionRepository,
            IRepository<UserGroup> userGroupRepository,
            IProtectedDocumentGroupRepository protectedDocumentGroupRepository,
            IPageSecurityRepository pageSecurityRepository
            )
        {
            _groupPermissionRepository = groupPermissionRepository;
            _userGroupRepository = userGroupRepository;
            _protectedDocumentGroupRepository = protectedDocumentGroupRepository;
            _pageSecurityRepository = pageSecurityRepository;
        }

        #region Validation

        /// <summary>
        /// Check if group name exists
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsNameExisted(int? id, string name)
        {
            return Fetch(u => u.Name.Equals(name) && u.Id != id).Any();
        }

        #endregion

        /// <summary>
        /// Gets the user groups.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetUserGroups(List<int> groupIds = null)
        {
            if (groupIds == null) groupIds = new List<int>();
            return GetAll().Select(g => new SelectListItem
            {
                Text = g.Name,
                Value = SqlFunctions.StringConvert((double) g.Id).Trim(),
                Selected = groupIds.Any(id => id == g.Id)
            });
        }

        /// <summary>
        /// Remove toolbar from user group
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public ResponseModel RemoveToolbarFromUserGroup(int userGroupId)
        {
            var userGroup = GetById(userGroupId);

            if (userGroup != null)
            {
                userGroup.ToolbarId = null;
                var response = Update(userGroup);

                return response.Success
                    ? response.SetMessage(T("UserGroupToolbar_Message_RemoveSuccessfully"))
                    : response.SetMessage(T("UserGroupToolbar_Message_RemoveFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("UserGroupToolbar_Message_RemoveSuccessfully")
            };
        }

        /// <summary>
        /// Get group by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UserGroup GetByName(string name)
        {
            return FetchFirst(g => g.Name.Equals(name));
        }

        #region Base

        public IQueryable<UserGroup> GetAll()
        {
            return _userGroupRepository.GetAll();
        }

        public IQueryable<UserGroup> Fetch(Expression<Func<UserGroup, bool>> expression)
        {
            return _userGroupRepository.Fetch(expression);
        }

        public UserGroup FetchFirst(Expression<Func<UserGroup, bool>> expression)
        {
            return _userGroupRepository.FetchFirst(expression);
        }

        public UserGroup GetById(object id)
        {
            return _userGroupRepository.GetById(id);
        }

        public ResponseModel Insert(UserGroup userGroup)
        {
            return _userGroupRepository.Insert(userGroup);
        }

        public ResponseModel Insert(IEnumerable<UserGroup> userGroups)
        {
            return _userGroupRepository.Insert(userGroups);
        }

        internal ResponseModel Update(UserGroup userGroup)
        {
            return _userGroupRepository.Update(userGroup);
        }

        internal ResponseModel Delete(UserGroup userGroup)
        {
            return _userGroupRepository.Delete(userGroup);
        }

        internal ResponseModel Delete(object id)
        {
            return _userGroupRepository.Delete(id);
        }

        #endregion

        #region Grid

        /// <summary>
        /// Search user groups by toolbar id
        /// </summary>
        /// <param name="si"></param>
        /// <param name="toolbarId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchUserGroups(JqSearchIn si, int? toolbarId)
        {
            var data = SearchUserGroups(toolbarId);

            var userGroups = Maps(data);

            return si.Search(userGroups);
        }

        /// <summary>
        /// Export user groups
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="toolbarId"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? toolbarId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchUserGroups(toolbarId);

            var userGroups = Maps(data);

            var exportData = si.Export(userGroups, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Search user groups by user id
        /// </summary>
        /// <param name="si"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchUserGroupsOfUser(JqSearchIn si, int userId)
        {
            var data = SearchUserGroupsOfUser(userId);

            var userGroups = Maps(data);

            return si.Search(userGroups);
        }

        /// <summary>
        /// Export user groups
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public HSSFWorkbook ExportsUserGroupsOfUser(JqSearchIn si, GridExportMode gridExportMode, int userId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchUserGroupsOfUser(userId);

            var userGroups = Maps(data);

            var exportData = si.Export(userGroups, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search user groups by toolbar id
        /// </summary>
        /// <param name="toolbarId"></param>
        /// <returns></returns>
        private IQueryable<UserGroup> SearchUserGroups(int? toolbarId)
        {
            return Fetch(usergroup => !toolbarId.HasValue || usergroup.ToolbarId == toolbarId);
        }

        /// <summary>
        /// Search user groups by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IQueryable<UserGroup> SearchUserGroupsOfUser(int userId)
        {
            return Fetch(usergroup => usergroup.UserUserGroups.Any(g => g.UserId == userId));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="userGroups"></param>
        /// <returns></returns>
        private IQueryable<UserGroupModel> Maps(IQueryable<UserGroup> userGroups)
        {
            return userGroups.ToList().Select(userGroup => new UserGroupModel(userGroup)).AsQueryable();
        }

        #endregion

        #endregion

        #region Permissions

        /// <summary>
        /// Gets permission setting of user group
        /// </summary>
        /// <param name="id">the user group id</param>
        /// <returns></returns>
        public GroupPermissionsModel GetPermissionSettings(int id)
        {
            var userGroup = GetById(id);
            if (userGroup == null)
                return null;

            var permissionIds = Enum.GetValues(typeof (Permission)).Cast<int>();

            var currentUserPermission = _groupPermissionRepository.GetByGroupId(id).ToList();

            foreach (var permissionId in permissionIds)
            {
                if (currentUserPermission.All(p => p.PermissionId != permissionId))
                {
                    var groupPermission = new GroupPermission
                    {
                        PermissionId = permissionId,
                        UserGroupId = id,
                        HasPermission = false
                    };
                    _groupPermissionRepository.Insert(groupPermission);
                }
            }

            var userPermissions =
                _groupPermissionRepository.GetByGroupId(id).ToList().Select(p => new GroupPermissionItem
                {
                    GroupPermissionId = p.Id,
                    PermissionName = ((Permission) p.PermissionId).GetEnumName(),
                    HasPermission = p.HasPermission
                }).ToList();
            return new GroupPermissionsModel
            {
                Name = userGroup.Name,
                Permissions = userPermissions
            };
        }

        /// <summary>
        /// Save permissions
        /// </summary>
        /// <param name="permissionIds">the right permission of user group</param>
        /// <param name="userGroupId">user group id</param>
        /// <returns></returns>
        public ResponseModel SavePermissions(List<int> permissionIds, int userGroupId)
        {
            var currentUserPermission = _groupPermissionRepository.GetByGroupId(userGroupId).ToList();
            foreach (var groupPermission in currentUserPermission)
            {
                if (permissionIds.Contains(groupPermission.Id))
                {
                    if (!groupPermission.HasPermission)
                    {
                        groupPermission.HasPermission = true;
                        _groupPermissionRepository.Update(groupPermission);
                    }
                }
                else if (groupPermission.HasPermission)
                {
                    groupPermission.HasPermission = false;
                    _groupPermissionRepository.Update(groupPermission);
                }
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("UserGroup_Message_UpdatePermissionsSuccessfully")
            };
        }

        /// <summary>
        /// Get all permissions of group
        /// </summary>
        /// <param name="id">the user group id</param>
        /// <returns></returns>
        public List<int> GetGroupPermissions(int id)
        {
            var userGroup = GetById(id);
            if (userGroup == null)
                return null;

            return userGroup.GroupPermissions.Select(p => p.PermissionId).ToList();
        }

        /// <summary>
        /// Check if group has permissions
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public bool HasPermissions(int groupId, params Permission[] permissions)
        {
            var groupPermissions = GetGroupPermissions(groupId);

            var requiredPermissions = permissions != null ? permissions.Select(p => (int) p).ToList() : new List<int>();

            return groupPermissions.Intersect(requiredPermissions).Count() == requiredPermissions.Count();
        }

        /// <summary>
        /// Check if group has one of permissions
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public bool HasOneOfPermissions(int groupId, params Permission[] permissions)
        {
            var modePermissions = GetGroupPermissions(groupId);

            var requiredPermissions = permissions != null ? permissions.Select(p => (int) p).ToList() : new List<int>();

            return modePermissions.Intersect(requiredPermissions).Any();
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get User Group manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserGroupManageModel GetUserGroupManageModel(int? id = null)
        {
            var userGroup = GetById(id);
            if (userGroup != null)
            {
                return new UserGroupManageModel(userGroup);
            }
            return new UserGroupManageModel();
        }

        /// <summary>
        /// Save User Group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveUserGroupManageModel(UserGroupManageModel model)
        {
            ResponseModel response;

            var userGroup = GetById(model.Id);
            if (userGroup != null)
            {
                userGroup.Name = model.Name;
                userGroup.Description = model.Description;
                userGroup.RedirectUrl = model.RedirectUrl;
                userGroup.ToolbarId = model.ToolbarId;
                userGroup.RecordOrder = model.RecordOrder;
                response = Update(userGroup);
                return response.SetMessage(response.Success
                    ? T("UserGroup_Message_UpdateSuccessfully")
                    : T("UserGroup_Message_UpdateFailure"));
            }

            Mapper.CreateMap<UserGroupManageModel, UserGroup>();
            userGroup = Mapper.Map<UserGroupManageModel, UserGroup>(model);

            response = Insert(userGroup);
            return response.SetMessage(response.Success
                ? T("UserGroup_Message_CreateSuccessfully")
                : T("UserGroup_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete user group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteUserGroup(int id)
        {
            var userGroup = GetById(id);
            if (userGroup != null)
            {
                if (userGroup.UserUserGroups.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("UserGroup_Message_DeleteFailureBasedOnRelatedUsers")
                    };
                }

                if (userGroup.PageSecurities.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("UserGroup_Message_DeleteFailureBasedOnRelatedPageSecurities")
                    };
                }

                if (userGroup.ProtectedDocumentGroups.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("UserGroup_Message_DeleteFailureBasedOnRelatedProtectedDocumentGroups")
                    };
                }

                //delete related group permission
                _groupPermissionRepository.Delete(userGroup.GroupPermissions);

                var response = Delete(userGroup);
                return response.SetMessage(response.Success
                    ? T("UserGroup_Message_DeleteSuccessfully")
                    : T("UserGroup_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("UserGroup_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get user group detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserGroupDetailModel GetUserGroupDetailModel(int id)
        {
            var userGroup = GetById(id);
            return userGroup != null ? new UserGroupDetailModel(userGroup) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateUserGroupData(XEditableModel model)
        {
            var userGroup = GetById(model.Pk);
            if (userGroup != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (UserGroupManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new UserGroupManageModel(userGroup);
                    manageModel.SetProperty(model.Name, value);

                    var validationResults = manageModel.ValidateModel();

                    if (validationResults.Any())
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = validationResults.BuildValidationMessages()
                        };
                    }

                    #endregion

                    userGroup.SetProperty(model.Name, value);
                    var response = Update(userGroup);
                    return response.SetMessage(response.Success
                        ? T("UserGroup_Message_UpdateUserGroupInfoSuccessfully")
                        : T("UserGroup_Message_UpdateUserGroupInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("UserGroup_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("UserGroup_Message_ObjectNotFound")
            };
        }

        #region Methods

        /// <summary>
        /// Search protected documents
        /// </summary>
        /// <param name="si"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchProtectedDocuments(JqSearchIn si, int userGroupId)
        {
            var data = _protectedDocumentGroupRepository.Fetch(p => p.UserGroupId == userGroupId);
            var protectedDocuments = data.Select(protectDocument => new ProtectedDocumentModel
            {
                Id = protectDocument.ProtectedDocumentId,
                Path = protectDocument.ProtectedDocument.Path
            });

            return si.Search(protectedDocuments);
        }

        /// <summary>
        /// Search group permission
        /// </summary>
        /// <param name="si"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchGroupPermissions(JqSearchIn si, int userGroupId)
        {
            var groupPermissionModel = GetPermissionSettings(userGroupId);
            var permissions = groupPermissionModel.Permissions.Where(m => m.HasPermission).AsQueryable();

            return si.Search(permissions);
        }

        /// <summary>
        /// Search page securities
        /// </summary>
        /// <param name="si"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchPageSecurities(JqSearchIn si, int userGroupId)
        {
            var data = _pageSecurityRepository.Fetch(g => g.GroupId == userGroupId);
            var pageSercurities = data.Select(pageSecurity => new SecurityModel
            {
                Id = pageSecurity.Id,
                PageId = pageSecurity.PageId,
                PageTitle = pageSecurity.Page.Title,
                CanEdit = pageSecurity.CanEdit,
                CanView = pageSecurity.CanView
            });

            return si.Search(pageSercurities);
        }

        /// <summary>
        /// Delete mapping between user group and protected document
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="protectedDocumentId"></param>
        /// <returns></returns>
        public ResponseModel DeleteUserGroupProtectedDocumentMapping(int userGroupId, int protectedDocumentId)
        {
            var response = _protectedDocumentGroupRepository.Delete(protectedDocumentId, userGroupId);

            return response.SetMessage(response.Success
                ? T("UserGroupProtectedDocument_Message_DeleteMappingSuccessfully")
                : T("UserGroupProtectedDocument_Message_DeleteMappingFailure"));
        }

        /// <summary>
        /// Delete mapping between user group and page security
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ResponseModel DeleteUserGroupPageSecurityMapping(int userGroupId, int pageId)
        {
            var response = _pageSecurityRepository.Delete(userGroupId, pageId);

            return response.SetMessage(response.Success
                ? T("UserGroupPageSecurity_Message_DeleteMappingSuccessfully")
                : T("UserGroupPageSecurity_Message_DeleteMappingFailure"));
        }

        #endregion

        #endregion
    }
}