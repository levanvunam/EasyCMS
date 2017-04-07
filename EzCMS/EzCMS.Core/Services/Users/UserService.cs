using AutoMapper;
using Ez.Framework.Configurations;
using Ez.Framework.Core.Context;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Files;
using Ez.Framework.Utilities.Reflection;
using Ez.Framework.Utilities.Web;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Exceptions.Users;
using EzCMS.Core.Framework.Plugins.Users;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Models.Users.Emails;
using EzCMS.Core.Models.Users.ExtendExpirationDate;
using EzCMS.Core.Models.Users.Logins;
using EzCMS.Core.Models.Users.Remotes;
using EzCMS.Core.Models.Users.Settings;
using EzCMS.Core.Services.Contacts;
using EzCMS.Core.Services.EmailLogs;
using EzCMS.Core.Services.EmailTemplates;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Core.Services.UserGroups;
using EzCMS.Core.Services.UserLoginHistories;
using EzCMS.Core.Services.Users.RemoteUsers;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.Users;
using EzCMS.Entity.Repositories.UserUserGroups;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EzCMS.Core.Services.Users
{
    public class UserService : ServiceHelper, IUserService
    {
        private readonly IContactService _contactService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IRemoteUserService _remoteUserService;
        private readonly ISiteSettingService _siteSettingService;
        private readonly IUserGroupService _userGroupService;
        private readonly IUserLoginHistoryService _userLoginHistoryService;
        private readonly IUserRepository _userRepository;
        private readonly IUserUserGroupRepository _userUserGroupRepository;

        public UserService(IUserGroupService userGroupService, ISiteSettingService siteSettingService,
            IRemoteUserService remoteUserService,
            IEmailTemplateService emailTemplateService, IEmailLogService emailLogService,
            IUserRepository userRepository, IUserUserGroupRepository userUserGroupRepository,
            IContactService contactService,
            IUserLoginHistoryService userLoginHistoryService)
        {
            _userGroupService = userGroupService;
            _siteSettingService = siteSettingService;
            _remoteUserService = remoteUserService;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;

            _userRepository = userRepository;
            _userUserGroupRepository = userUserGroupRepository;
            _contactService = contactService;
            _userLoginHistoryService = userLoginHistoryService;
        }

        #region Validation

        /// <summary>
        /// Check if user exists
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool IsUsernameExisted(int? userId, string username)
        {
            return
                Fetch(
                    user =>
                        (string.IsNullOrEmpty(user.Username)
                            ? user.Email.Equals(username)
                            : user.Username.Equals(username)) &&
                        user.Id != userId).Any();
        }

        #endregion

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="isRemote">specify if only look for remote or local or any type of accounts</param>
        /// <returns></returns>
        public User GetByUsername(string username, bool? isRemote = null)
        {
            return _userRepository.GetByUsername(username, isRemote);
        }

        /// <summary>
        /// Get active user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public User GetActiveUser(string username)
        {
            return _userRepository.GetActiveUser(username);
        }

        /// <summary>
        /// Get user profile from username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public UserProfileModel GetUserProfile(string username)
        {
            var user = GetActiveUser(username);

            return new UserProfileModel(user);
        }

        /// <summary>
        /// Get users
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetUsers(int? userId = null)
        {
            return GetAll().Select(user => new SelectListItem
            {
                Text = string.IsNullOrEmpty(user.Username) ? user.Email : user.Username,
                Value = SqlFunctions.StringConvert((double)user.Id).Trim(),
                Selected = userId.HasValue && user.Id == userId
            });
        }

        /// <summary>
        /// Check user will expire or not
        /// </summary>
        /// <param name="userGroupIds"></param>
        /// <param name="isAdministrator"></param>
        /// <returns></returns>
        private bool IsExpirableUser(List<int> userGroupIds, bool isAdministrator = false)
        {
            var accountExpiresSetting = _siteSettingService.LoadSetting<AccountExpiredSetting>();
            if (isAdministrator || accountExpiresSetting.NumberOfDaysToKeepAccountAlive <= 0) return false;

            if (userGroupIds == null || !userGroupIds.Any())
            {
                return false;
            }

            var expirableGroupIds = accountExpiresSetting.ExpirableUserGroupIds;
            if (expirableGroupIds == null || !expirableGroupIds.Any())
            {
                return false;
            }
            return expirableGroupIds.Any(userGroupIds.Contains);
        }

        #region Base

        public IQueryable<User> GetAll()
        {
            return _userRepository.GetAll();
        }

        public IQueryable<User> Fetch(Expression<Func<User, bool>> expression)
        {
            return _userRepository.Fetch(expression);
        }

        public User FetchFirst(Expression<Func<User, bool>> expression)
        {
            return _userRepository.FetchFirst(expression);
        }

        public User GetById(object id)
        {
            return _userRepository.GetById(id);
        }

        public ResponseModel Delete(User user)
        {
            return _userRepository.Delete(user);
        }

        internal ResponseModel Delete(object id)
        {
            return _userRepository.Delete(id);
        }

        #endregion

        #region Grid

        /// <summary>
        /// Search the users
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchUsers(JqSearchIn si, UserSearchModel model)
        {
            var data = SearchUsers(model);

            var users = Maps(data);

            return si.Search(users);
        }

        /// <summary>
        /// Export users
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, UserSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchUsers(model);

            var users = Maps(data);

            var exportData = si.Export(users, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search users
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<User> SearchUsers(UserSearchModel model)
        {
            if (model == null) return GetAll();
            var users =
                Fetch(
                    u => (!model.UserGroupId.HasValue || u.UserUserGroups.Any(l => l.UserGroupId == model.UserGroupId))
                         && (string.IsNullOrEmpty(model.Keyword)
                             || (!string.IsNullOrEmpty(u.FirstName) && u.FirstName.Contains(model.Keyword))
                             || (!string.IsNullOrEmpty(u.LastName) && u.LastName.Contains(model.Keyword))
                             || (!string.IsNullOrEmpty(u.Email) && u.Email.Contains(model.Keyword))
                             || (!string.IsNullOrEmpty(u.Username) && u.Username.Contains(model.Keyword))
                             || (!string.IsNullOrEmpty(u.Phone) && u.Phone.Contains(model.Keyword))
                             || (!string.IsNullOrEmpty(u.Address) && u.Address.Contains(model.Keyword))
                             || (!string.IsNullOrEmpty(u.About) && u.About.Contains(model.Keyword))
                             || (!string.IsNullOrEmpty(u.Facebook) && u.Facebook.Contains(model.Keyword))
                             || (!string.IsNullOrEmpty(u.Twitter) && u.Twitter.Contains(model.Keyword))
                             || (!string.IsNullOrEmpty(u.LinkedIn) && u.LinkedIn.Contains(model.Keyword))));
            return users;
        }

        /// <summary>
        /// Map entities to models
        /// Map user to user model
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        private IQueryable<UserModel> Maps(IQueryable<User> users)
        {
            return users.Select(u => new UserModel
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserGroupNames = u.UserUserGroups.Select(x => x.UserGroup.Name),
                Status = u.Status,
                IsSystemAdministrator = u.IsSystemAdministrator,
                IsRemoteAccount = u.IsRemoteAccount,
                LoginTimes = u.UserLoginHistories.Count,
                LastLoginId =
                    u.UserLoginHistories.Any()
                        ? u.UserLoginHistories.OrderByDescending(ulh => ulh.Created).FirstOrDefault().Id
                        : (int?)null,
                LastLogin =
                    u.UserLoginHistories.Any()
                        ? u.UserLoginHistories.OrderByDescending(ulh => ulh.Created).FirstOrDefault().Created
                        : (DateTime?)null,
                AvatarFileName = u.AvatarFileName,
                Phone = u.Phone,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                About = u.About,
                ReleaseLockDate = u.ReleaseLockDate,
                LastPasswordChange = u.LastPasswordChange,
                PasswordFailsCount = u.PasswordFailsCount,
                LastFailedLogin = u.LastFailedLogin,
                AccountExpiresDate = u.AccountExpiresDate,
                Address = u.Address,
                Facebook = u.Facebook,
                Twitter = u.Twitter,
                LinkedIn = u.LinkedIn,
                Password = u.Password,
                RecordOrder = u.RecordOrder,
                Created = u.Created,
                CreatedBy = u.CreatedBy,
                LastUpdate = u.LastUpdate,
                LastUpdateBy = u.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        #region Contact User

        /// <summary>
        /// Get user create model for contact
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public ContactUserManageModel GetContactUserManageModel(int contactId)
        {
            var contact = _contactService.GetById(contactId);

            if (contact != null)
            {
                if (!contact.UserId.HasValue)
                {
                    return new ContactUserManageModel(contact);
                }
            }

            return null;
        }

        /// <summary>
        /// Save user from contact
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveContactUserManageModel(ContactUserManageModel model)
        {
            var userManageModel = new UserManageModel(model);

            return SaveUserManageModel(userManageModel, null, model.ContactId);
        }

        #endregion

        #region Full User

        /// <summary>
        /// Get user manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserManageModel GetUserManageModel(int? id = null)
        {
            var user = GetById(id);
            if (user != null)
            {
                return new UserManageModel(user);
            }

            return new UserManageModel();
        }

        /// <summary>
        /// Save user manage model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="avatar"></param>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public ResponseModel SaveUserManageModel(UserManageModel model, HttpPostedFileBase avatar = null,
            int? contactId = null)
        {
            ResponseModel response;
            var user = GetById(model.Id);

            string saltKey, hashPass;
            var rawPassword = model.Password;
            model.Password.GeneratePassword(out saltKey, out hashPass);

            var accountExpiresSetting = _siteSettingService.LoadSetting<AccountExpiredSetting>();

            #region Edit User

            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                // Only set administrator if current user is administrator
                if (WorkContext.CurrentUser.IsSystemAdministrator)
                {
                    user.IsSystemAdministrator = model.IsSystemAdministrator;
                }

                //user.Username = model.Username;
                //user.Email = model.Email;
                user.Phone = model.Phone;
                user.Gender = model.Gender;
                user.About = model.About;
                user.Address = model.Address;
                user.Facebook = model.Facebook;
                user.Twitter = model.Twitter;
                user.LinkedIn = model.LinkedIn;
                user.AvatarFileName = model.AvatarFileName;
                user.DateOfBirth = model.DateOfBirth;
                user.Settings = SerializeUtilities.Serialize(model.ManageSettingModel);
                user.ChangePasswordAfterLogin = model.ChangePasswordAfterLogin;

                // Password change
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    if (user.IsRemoteAccount)
                    {
                        try
                        {
                            _remoteUserService.ResetPassword(
                                string.IsNullOrEmpty(user.Username) ? user.Email : user.Username, model.Password);
                        }
                        catch (ServiceNotAvailableException)
                        {
                            return new ResponseModel
                            {
                                Success = false,
                                Message = T("User_Message_RemoteServiceNotAvailable")
                            };
                        }
                    }
                    else
                    {
                        //update password and passwordsalt
                        user.PasswordSalt = saltKey;
                        user.Password = hashPass;
                    }
                }

                #region User Groups

                if (!user.IsRemoteAccount)
                {
                    var currentGroups = user.UserUserGroups.Select(nc => nc.UserGroupId).ToList();

                    if (model.UserGroupIds == null) model.UserGroupIds = new List<int>();

                    //Delete groups
                    var removedGroupIds = currentGroups.Where(id => !model.UserGroupIds.Contains(id));
                    _userUserGroupRepository.Delete(user.Id, removedGroupIds);

                    //Add new groups
                    var addedGroupIds = model.UserGroupIds.Where(id => !currentGroups.Contains(id));
                    _userUserGroupRepository.Insert(user.Id, addedGroupIds);
                }

                #endregion

                // Active user
                if (user.Status == UserEnums.UserStatus.Disabled && model.Status == UserEnums.UserStatus.Active)
                {
                    if (!IsExpirableUser(model.UserGroupIds, model.IsSystemAdministrator))
                    {
                        user.AccountExpiresDate = null;
                    }
                    else
                    {
                        if (accountExpiresSetting.NumberOfDaysToKeepAccountAlive > 0)
                        {
                            user.AccountExpiresDate =
                                DateTime.UtcNow.AddDays(accountExpiresSetting.NumberOfDaysToKeepAccountAlive);
                        }
                        else
                        {
                            user.AccountExpiresDate = null;
                        }
                    }
                }

                // Disable user
                if (user.Status == UserEnums.UserStatus.Active && model.Status == UserEnums.UserStatus.Disabled)
                {
                    if (!IsExpirableUser(model.UserGroupIds, model.IsSystemAdministrator))
                    {
                        user.AccountExpiresDate = DateTime.UtcNow;
                    }
                }

                user.Status = model.Status;

                response = UpdateUser(user);

                if (response.Success && avatar != null)
                {
                    UploadAvatar(user.Id, avatar);
                }

                return response.SetMessage(response.Success
                    ? T("User_Message_UpdateSuccessfully")
                    : T("User_Message_UpdateFailure"));
            }

            #endregion

            Mapper.CreateMap<UserManageModel, User>();
            user = Mapper.Map<UserManageModel, User>(model);
            user.Settings = SerializeUtilities.Serialize(model.ManageSettingModel);

            //create password and password salt
            user.PasswordSalt = saltKey;
            user.Password = hashPass;

            //Add account expires date for user
            if (IsExpirableUser(model.UserGroupIds, user.IsSystemAdministrator))
            {
                user.AccountExpiresDate = DateTime.UtcNow.AddDays(accountExpiresSetting.NumberOfDaysToKeepAccountAlive);
            }

            response = InsertUser(user, contactId);

            #region User Groups

            if (model.UserGroupIds != null)
            {
                _userUserGroupRepository.Insert(user.Id, model.UserGroupIds);
            }

            #endregion

            //Send notification to user
            SendUserNotification(user, rawPassword);

            if (response.Success && avatar != null)
            {
                UploadAvatar(user.Id, avatar);
            }

            return response.SetMessage(response.Success
                ? T("User_Message_CreateSuccessfully")
                : T("User_Message_CreateFailure"));
        }

        #endregion

        #region Simple User

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SimpleUserManageModel GetSimpleUserManageModel(int? id = null)
        {
            var user = GetById(id);
            if (user != null)
            {
                return new SimpleUserManageModel(user);
            }
            return new SimpleUserManageModel();
        }

        /// <summary>
        /// Save user manage model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveSimpleUserManageModel(SimpleUserManageModel model)
        {
            var userManageModel = new UserManageModel(model);

            return SaveUserManageModel(userManageModel);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Insert user and setup contact if needed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="contactId"></param>
        /// <returns></returns>
        private ResponseModel InsertUser(User user, int? contactId = null)
        {
            return _userRepository.InsertUser(user, contactId);
        }

        /// <summary>
        /// Send user created notification
        /// </summary>
        /// <param name="user"></param>
        /// <param name="rawPassword"></param>
        private void SendUserNotification(User user, string rawPassword = null)
        {
            #region Sending notification email

            user = GenerateResetPasswordCode(user);

            var loginLink = UrlUtilities.GenerateUrl(HttpContext.Current.Request.RequestContext, "Account", "Login", new
            {
                area = "Admin"
            }, true);

            var resetLink = UrlUtilities.GenerateUrl(HttpContext.Current.Request.RequestContext, "Account",
                "ResetPassword", new
                {
                    @userid = user.Id,
                    @code = user.ResetPasswordCode,
                    area = "Admin"
                }, true);

            var userCreatedEmailModel = new UserCreatedEmailModel
            {
                Username = string.IsNullOrEmpty(user.Username) ? user.Email : user.Username,
                Email = user.Email,
                Password = rawPassword,
                FullName = user.FullName,
                SiteUrl = EzWorkContext.SiteUrl,
                LoginLink = loginLink,
                ResetPasswordLink = resetLink
            };

            var emailResponse = _emailTemplateService.ParseEmail(EmailEnums.EmailTemplateType.UserCreatedNotification,
                userCreatedEmailModel);

            if (emailResponse != null)
            {
                var emailLog = new EmailLog
                {
                    From = emailResponse.From,
                    FromName = emailResponse.FromName,
                    CC = emailResponse.CC,
                    Bcc = emailResponse.BCC,
                    Subject = emailResponse.Subject,
                    Body = emailResponse.Body,
                    To = user.Email,
                    ToName = user.FullName,
                    Priority = EmailEnums.EmailPriority.Medium
                };

                _emailLogService.CreateEmail(emailLog, true);
            }

            #endregion
        }

        #endregion

        #region Update User

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResponseModel UpdateUser(User user)
        {
            return _userRepository.Update(user);
        }

        /// <summary>
        /// Update user data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateUserData(XEditableModel model)
        {
            var user = GetById(model.Pk);
            if (user != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof(User))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new UserManageModel(user);
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

                    user.SetProperty(model.Name, value);
                    var response = UpdateUser(user);

                    if (user.Id == WorkContext.CurrentUser.Id && response.Success)
                    {
                        UserSessionModel.SetUserSession(user);
                    }

                    return response.SetMessage(response.Success
                        ? T("User_Message_UpdateUserInfoSuccessfully")
                        : T("User_Message_UpdateUserInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("User_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("User_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Change user status
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        public ResponseModel ChangeUserStatus(int userId, UserEnums.UserStatus userStatus)
        {
            var user = GetById(userId);
            if (user != null)
            {
                user.Status = userStatus;
                var response = UpdateUser(user);
                return response.SetMessage(response.Success
                    ? T("User_Message_ChangeStatusSuccessfully")
                    : T("User_Message_ChangeStatusFailure"));
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("User_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteUser(int id)
        {
            var user = GetById(id);
            if (user != null)
            {
                if (WorkContext.CurrentUser.Id == id)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("User_Message_DeleteFailureBasedOnOwnAccount")
                    };
                }

                // Check existed contacts --> Remove UserId
                var contacts = _contactService.SearchContactsByUser(id).ToList();
                foreach (var contact in contacts)
                {
                    _contactService.DeleteContactUserMapping(contact.Id);
                }

                // Delete user in groups
                _userUserGroupRepository.Delete(id);

                // Set delete status of User
                var response = _userRepository.SetRecordDeleted(user.Id);

                return response.SetMessage(response.Success
                    ? T("User_Message_DeleteSuccessfully")
                    : T("User_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("User_Message_DeleteSuccessfully")
            };
        }

        #endregion

        /// <summary>
        /// Remove relationship between user and user group
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public ResponseModel DeleteUserUserGroupMapping(int userId, int userGroupId)
        {
            var response = _userUserGroupRepository.Delete(userId, userGroupId);

            return response.SetMessage(response.Success
                ? T("UserUserGroup_Message_DeleteMappingSuccessfully")
                : T("UserUserGroup_Message_DeleteMappingFailure"));
        }

        #endregion

        #region Login/ Register / Forgot Password/ Change Password

        #region Login

        /// <summary>
        /// Login method
        /// </summary>
        /// <param name="model">the login model</param>
        /// <returns></returns>
        public ResponseModel Login(LoginModel model)
        {
            try
            {
                // This is used when we open popup then should close it first, otherwise will return to login success page
                var isRedirectPermanent = false;

                dynamic session;
                var user = CheckLogin(model.Username, model.Password, out session);

                LoginFor(user, model.RememberMe, session);

                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

                var returnUrl = string.Empty;

                #region Check expiry password

                var userLoginSetting = _siteSettingService.LoadSetting<UserLoginSetting>();
                if (userLoginSetting.PasswordExpiryDays != 0 &&
                    user.LastPasswordChange.AddDays(userLoginSetting.PasswordExpiryDays) > DateTime.UtcNow)
                {
                    user.ChangePasswordAfterLogin = true;
                    UpdateUser(user);
                }

                #endregion

                #region Redirection

                /* The priority for redirection:
                     * * 1. Redirect URL based on the functionality from client module
                     * * 2. Redirect URL based on user group
                     * * 3. Redirect URL based on return url from incoming URL
                     * * 4. Redirect to Login Success
                     */

                //1. Redirect URL based on the functionality from client module
                var userLoginImplementations = HostContainer.GetInstances<IUserLoginRegister>();
                foreach (var userLoginImplementation in userLoginImplementations)
                {
                    returnUrl = userLoginImplementation.GetRedirectUrl();
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        isRedirectPermanent = true;
                        returnUrl = returnUrl.ToAbsoluteUrl();
                        break;
                    }
                }

                if (string.IsNullOrEmpty(returnUrl))
                {
                    //2. Redirect URL based on user group
                    var redirectGroupUrls = user.UserUserGroups != null
                        ? user.UserUserGroups.Where(
                            uig => uig.UserGroup != null && !string.IsNullOrEmpty(uig.UserGroup.RedirectUrl))
                            .Select(g => g.UserGroup.RedirectUrl)
                            .ToList()
                        : new List<string>();

                    if (redirectGroupUrls.Any())
                    {
                        foreach (var redirectGroupUrl in redirectGroupUrls)
                        {
                            returnUrl = redirectGroupUrl;

                            // Check if input url is local url
                            if (urlHelper.IsLocalUrl(returnUrl))
                            {
                                returnUrl = returnUrl.ToAbsoluteUrl();
                            }

                            var responseCode = returnUrl.GetResponseCode();

                            if (responseCode != HttpStatusCode.OK)
                            {
                                returnUrl = string.Empty;
                            }
                            else
                            {
                                isRedirectPermanent = true;
                                break;
                            }
                        }
                    }

                    //3. Redirect URL based on return url from incoming URL - must be local to avoid redirect attack
                    //4. Redirect to Login Success
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        if (string.IsNullOrEmpty(model.ReturnUrl) || !model.ReturnUrl.IsLocalUrl())
                        {
                            returnUrl = urlHelper.Action("LoginSuccess", "Account");
                        }
                        else
                        {
                            returnUrl = model.ReturnUrl;
                        }
                    }
                }

                if (user.ChangePasswordAfterLogin)
                {
                    returnUrl = urlHelper.Action("ChangePasswordAfterLogin", "Account", new { returnUrl });
                }

                #endregion

                return new ResponseModel
                {
                    Success = true,
                    Message = T("User_Message_LoginSuccessfully"),
                    Data = new
                    {
                        ReturnUrl = returnUrl,
                        IsRedirectPermanent = isRedirectPermanent
                    }
                };
            }
            catch (WrongUserNamePasswordException)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("User_Message_InvalidUserPassword")
                };
            }
            catch (AccountLockedException)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("User_Message_AccountLocked")
                };
            }
            catch (AccountInactiveException)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message =
                        T("User_Message_AccountInactive")
                };
            }
            catch (ServiceNotAvailableException)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("User_Message_RemoteServiceNotAvailable")
                };
            }
            catch (Exception exception)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("User_Message_InvalidUserPassword"),
                    DetailMessage = ResponseModel.BuildDetailMessage(exception)
                };
            }
        }

        /// <summary>
        /// Check if user/pass is corresponding to a valid identity (local or remote)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public User CheckLogin(string username, string password, out dynamic session)
        {
            session = null;
            var user =
                _userRepository.FetchFirst(
                    u => string.IsNullOrEmpty(u.Username) ? u.Email.Equals(username) : u.Username.Equals(username));
            if (user == null && _userRepository.GetAll().Count(u => u.Email.Equals(username)) == 1)
            {
                user = _userRepository.FetchFirst(u => u.Email.Equals(username));
            }

            // User is remote?
            if (user == null || user.IsRemoteAccount)
            {
                // Get remote user
                var remoteUser = GetRemoteUser(username);

                // Not existing -> wrong username
                if (remoteUser == null)
                {
                    throw new WrongUserNamePasswordException();
                }

                try
                {
                    session = remoteUser.Session;
                }
                catch
                {
                    // Cannot save the session
                }

                // Else -> Update remote user into local db
                if (user == null)
                {
                    // Insert remote user
                    user = InsertRemoteUser(remoteUser);
                }
                else
                {
                    // Update user groups
                    user = UpdateRemoteUser(user, remoteUser);
                }
            }

            // Inactive?
            if (user.Status == UserEnums.UserStatus.Disabled)
            {
                throw new AccountInactiveException();
            }

            // Locked?
            if (user.ReleaseLockDate > DateTime.UtcNow)
            {
                throw new AccountLockedException();
            }

            var isValidPassword = ValidateUserLogin(username, password, user.IsRemoteAccount);

            if (isValidPassword)
            {
                // Update user last login & clear failed login
                user.LastFailedLogin = null;
                user.PasswordFailsCount = 0;
                user.ReleaseLockDate = null;
                user.LastLogin = DateTime.UtcNow;
                user.ResetPasswordCode = null;
                user.ResetPasswordExpiryDate = null;

                UpdateUser(user);

                return user;
            }

            // Wrong password -> update last failed login, increase failed attempts
            user.LastFailedLogin = DateTime.UtcNow;
            user.PasswordFailsCount += 1;

            var userLoginSetting = _siteSettingService.LoadSetting<UserLoginSetting>();

            var maxAllowedLoginAttempts = userLoginSetting.MaxAllowedLoginAttempts;
            var lockOutTimeInMinutes = userLoginSetting.LockOutTimeInMinutes;

            if (user.PasswordFailsCount >= maxAllowedLoginAttempts)
            {
                user.ReleaseLockDate = DateTime.UtcNow.AddMinutes(lockOutTimeInMinutes);
                user.PasswordFailsCount = 0;
            }

            UpdateUser(user);

            // Throw wrong password exception
            throw new WrongUserNamePasswordException();
        }

        /// <summary>
        /// Check if user/pass is corresponding to a valid identity (local or remote)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User CheckLogin(string username, string password)
        {
            dynamic session;
            return CheckLogin(username, password, out session);
        }

        #region Private Methods

        /// <summary>
        /// Update remote user information
        /// </summary>
        /// <param name="user"></param>
        /// <param name="remoteUser"></param>
        /// <returns></returns>
        private User UpdateRemoteUser(User user, RemoteUser remoteUser)
        {
            #region Update user information

            user.FirstName = remoteUser.FirstName;
            user.LastName = remoteUser.LastName;
            user.IsSystemAdministrator = remoteUser.IsSystemAdministrator;

            #endregion

            if (remoteUser.UserGroups == null) remoteUser.UserGroups = new List<string>();

            #region Add not existed groups

            var availableGroups = _userGroupService.GetAll().Select(g => g.Name).ToList();
            var notExistedGroups = remoteUser.UserGroups.Where(g => !availableGroups.Contains(g));
            _userGroupService.Insert(notExistedGroups.Select(g => new UserGroup
            {
                Name = g
            }));

            #endregion

            //remove all user in group
            _userUserGroupRepository.Delete(user.Id);

            //get user in group from remote user
            var userGroupIds =
                _userGroupService.Fetch(g => remoteUser.UserGroups.Contains(g.Name)).Select(g => g.Id).ToList();

            //insert into user in group
            _userUserGroupRepository.Insert(user.Id, userGroupIds);

            return user;
        }

        /// <summary>
        /// Add new remote user
        /// </summary>
        /// <param name="remoteUser"></param>
        /// <returns></returns>
        private User InsertRemoteUser(RemoteUser remoteUser)
        {
            #region User groups

            // Insert not available groups
            var availableGroups = _userGroupService.GetAll().Select(g => g.Name).ToList();
            var notExistedGroups = remoteUser.UserGroups.Where(g => !availableGroups.Contains(g));
            _userGroupService.Insert(notExistedGroups.Select(name => new UserGroup
            {
                Name = name
            }));

            // Get group ids
            var existedGroups =
                _userGroupService.Fetch(g => remoteUser.UserGroups.Contains(g.Name)).Select(g => g.Id).ToList();

            var user = new SimpleUserManageModel
            {
                Username = remoteUser.Username,
                Email = remoteUser.Email,
                Password = string.Empty,
                FirstName = remoteUser.FirstName,
                LastName = remoteUser.LastName,
                Status = UserEnums.UserStatus.Active,
                IsSystemAdministrator = remoteUser.IsSystemAdministrator,
                IsRemoteAccount = true,
                UserGroupIds = existedGroups
            };

            var response = SaveSimpleUserManageModel(user);

            if (response.Success)
            {
                return GetById(response.Data);
            }

            #endregion

            return null;
        }

        /// <summary>
        /// Validate user login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="isRemote"></param>
        /// <returns></returns>
        private bool ValidateUserLogin(string username, string password, bool isRemote)
        {
            if (isRemote) return CheckRemoteLogin(username, password);

            return CheckLocalLogin(username, password);
        }

        /// <summary>
        /// Get remote user by user name
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private RemoteUser GetRemoteUser(string username)
        {
            return _remoteUserService.GetRemoteUser(username);
        }

        /// <summary>
        /// Generate the reset password code
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private User GenerateResetPasswordCode(User user)
        {
            var userLoginSetting = _siteSettingService.LoadSetting<UserLoginSetting>();

            var resetPasswordEffectiveTimeInMinutes = userLoginSetting.ResetPasswordEffectiveTimeInMinutes;
            user.ResetPasswordCode = Guid.NewGuid().ToString();
            user.ResetPasswordExpiryDate = DateTime.UtcNow.AddMinutes(resetPasswordEffectiveTimeInMinutes);

            UpdateUser(user);

            return user;
        }

        /// <summary>
        /// Check if user/pass is corresponding to a valid local identity
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool CheckLocalLogin(string username, string password)
        {
            var user = GetByUsername(username, false);

            if (user != null)
            {
                var result = PasswordUtilities.ValidatePass(user.Password, password, user.PasswordSalt);
                return result;
                //return user.Password == password;
            }

            return false;
        }

        /// <summary>
        /// Check if user/pass is corresponding to a valid remote identity
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool CheckRemoteLogin(string username, string password)
        {
            return _remoteUserService.ValidateUsernamePassword(username, password);
        }

        /// <summary>
        /// Make any work needed for the system to recognize current user as authenticated.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="rememberMe"></param>
        /// <param name="session"></param>
        private void LoginFor(User user, bool rememberMe, dynamic session)
        {
            var username = string.IsNullOrEmpty(user.Username) ? user.Email : user.Username;
            if (rememberMe)
            {
                var authenticationTicket = new FormsAuthenticationTicket(
                    1,
                    username,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddMonths(1),
                    true,
                    username,
                    "/"
                    );

                //encrypt the ticket and add it to a cookie
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                    FormsAuthentication.Encrypt(authenticationTicket));
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            FormsAuthentication.SetAuthCookie(username, rememberMe);

            // Save this information for getting updates
            var lastTimeGettingUpdate = user.LastLogin;

            user.LastLogin = DateTime.UtcNow;
            UpdateUser(user);

            // Add login history for user
            _userLoginHistoryService.InsertUserLoginHistory(user.Id, HttpContext.Current);

            UserSessionModel.SetUserSession(user, lastTimeGettingUpdate, session);
        }

        #endregion

        #endregion

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel ForgotPassword(ForgotPasswordModel model)
        {
            var user = GetActiveUser(model.Username);
            if (user != null)
            {
                user = GenerateResetPasswordCode(user);

                var resetLink = UrlUtilities.GenerateUrl(HttpContext.Current.Request.RequestContext, "Account",
                    "ResetPassword", new { @userid = user.Id, @code = user.ResetPasswordCode, area = "Admin" }, true);

                var userLoginSetting = _siteSettingService.LoadSetting<UserLoginSetting>();

                var resetPasswordEffectiveTimeInMinutes = userLoginSetting.ResetPasswordEffectiveTimeInMinutes;
                var forgotEmailModel = new ForgotPasswordEmailModel
                {
                    Username = string.IsNullOrEmpty(user.Username) ? user.Email : user.Username,
                    FullName = user.FullName,
                    ResetLink = resetLink,
                    EffectiveTimeInMinutes = resetPasswordEffectiveTimeInMinutes
                };

                var emailResponse = _emailTemplateService.ParseEmail(EmailEnums.EmailTemplateType.ForgotPassword,
                    forgotEmailModel);

                if (emailResponse == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("User_Message_MissingForgotPasswordEmailTemplate")
                    };
                }

                var emailLog = new EmailLog
                {
                    From = emailResponse.From,
                    FromName = emailResponse.FromName,
                    CC = emailResponse.CC,
                    Bcc = emailResponse.BCC,
                    Subject = emailResponse.Subject,
                    Body = emailResponse.Body,
                    To = user.Email,
                    ToName = user.FullName,
                    Priority = EmailEnums.EmailPriority.Medium
                };

                var response = _emailLogService.CreateEmail(emailLog, true);

                return response.Success
                    ? response.SetMessage(T("User_Message_ForgotPasswordActionSuccessfully"))
                    : response.SetMessage(T("User_Message_ForgotPasswordActionFailure"));
            }

            // Even if email not existed, we still notify that we sent the email already
            return new ResponseModel
            {
                Success = true,
                Message = T("User_Message_ForgotPasswordActionSuccessfully")
            };
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel ResetPassword(ResetPasswordModel model)
        {
            var user = GetById(model.UserId);
            if (user != null)
            {
                if (user.IsRemoteAccount)
                {
                    var username = string.IsNullOrEmpty(user.Username) ? user.Email : user.Username;
                    var successful = _remoteUserService.ResetPassword(username, model.Password);
                    if (!successful)
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = T("User_Message_UnableToChangeRemotePassword")
                        };
                    }
                }
                else
                {
                    string saltKey, hashPass;
                    model.Password.GeneratePassword(out saltKey, out hashPass);

                    user.PasswordSalt = saltKey;
                    user.Password = hashPass;
                }

                // Update user status
                user.LastPasswordChange = DateTime.UtcNow;
                user.ResetPasswordCode = null;
                user.ResetPasswordExpiryDate = null;
                user.ChangePasswordAfterLogin = false;

                UpdateUser(user);

                return new ResponseModel
                {
                    Success = true,
                    Message = T("User_Message_ChangePasswordSuccessfully")
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("User_Message_UserDoesNotExist")
            };
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel Register(RegisterModel model)
        {
            #region Add user to db

            string saltKey, hashPass;
            model.Password.GeneratePassword(out saltKey, out hashPass);

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = hashPass,
                PasswordSalt = saltKey,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Status = UserEnums.UserStatus.Active
            };

            var response = InsertUser(user);

            #endregion

            #region Sending register notification email

            var loginLink = UrlUtilities.GenerateUrl(HttpContext.Current.Request.RequestContext, "Account", "Login", new
            {
                area = "Admin"
            }, true);

            var registerEmailModel = new RegisterEmailModel
            {
                Username = string.IsNullOrEmpty(user.Username) ? user.Email : user.Username,
                FullName = user.FullName,
                LoginLink = loginLink,
                SiteUrl = EzWorkContext.SiteUrl
            };

            var emailResponse = _emailTemplateService.ParseEmail(EmailEnums.EmailTemplateType.RegisterUser,
                registerEmailModel);

            if (emailResponse != null)
            {
                var emailLog = new EmailLog
                {
                    From = emailResponse.From,
                    FromName = emailResponse.FromName,
                    CC = emailResponse.CC,
                    Bcc = emailResponse.BCC,
                    Subject = emailResponse.Subject,
                    Body = emailResponse.Body,
                    To = user.Email,
                    ToName = user.FullName,
                    Priority = EmailEnums.EmailPriority.Medium
                };

                _emailLogService.CreateEmail(emailLog, true);
            }

            #endregion

            response = response.Success
                ? response.SetMessage(T("User_Message_RegisterSuccessfully"))
                : response.SetMessage(T("User_Message_RegisterFailure"));

            return response;
        }

        #region Change Password

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="model">the change password model</param>
        /// <returns></returns>
        public ResponseModel ChangePassword(ChangePasswordModel model)
        {
            var user = GetById(model.UserId);
            if (user != null)
            {
                if (user.IsRemoteAccount)
                {
                    try
                    {
                        var username = string.IsNullOrEmpty(user.Username) ? user.Email : user.Username;
                        _remoteUserService.ChangePassword(username, model.OldPassword, model.Password);
                    }
                    catch (ServiceNotAvailableException)
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = T("User_Message_RemoteServiceNotAvailable")
                        };
                    }
                }
                else
                {
                    string saltKey, hashPass;
                    model.Password.GeneratePassword(out saltKey, out hashPass);

                    user.Password = hashPass;
                    user.PasswordSalt = saltKey;
                }

                user.ChangePasswordAfterLogin = false;
                user.LastPasswordChange = DateTime.UtcNow;

                var response = UpdateUser(user);
                return response.SetMessage(response.Success
                    ? T("User_Message_ChangePasswordSuccessfully")
                    : T("User_Message_ChangePasswordFailure"));
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("User_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Check reset password code
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsResetPasswordCodeValid(int userId, string code)
        {
            var user = GetById(userId);

            return user != null
                   && user.Status == UserEnums.UserStatus.Active
                   && !string.IsNullOrWhiteSpace(user.ResetPasswordCode)
                   && user.ResetPasswordExpiryDate > DateTime.UtcNow
                   && user.ResetPasswordCode.Equals(code, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Change password affter login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel ChangePasswordAfterLogin(ChangePasswordAfterLoginModel model)
        {
            var user = GetById(model.UserId);
            if (user != null)
            {
                if (user.IsRemoteAccount)
                {
                    var username = string.IsNullOrEmpty(user.Username) ? user.Email : user.Username;
                    var successful = _remoteUserService.ResetPassword(username, model.Password);
                    if (!successful)
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = T("User_Message_UnableToChangeRemotePassword")
                        };
                    }
                }
                else
                {
                    string saltKey, hashPass;
                    model.Password.GeneratePassword(out saltKey, out hashPass);

                    user.Password = hashPass;
                    user.PasswordSalt = saltKey;
                }

                user.LastPasswordChange = DateTime.UtcNow;
                user.ResetPasswordCode = null;
                user.ResetPasswordExpiryDate = null;
                user.ChangePasswordAfterLogin = false;

                UpdateUser(user);

                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

                return new ResponseModel
                {
                    Success = true,
                    Message = T("User_Message_ChangePasswordSuccessfully"),
                    Data = string.IsNullOrEmpty(model.ReturnUrl)
                        ? urlHelper.Action("LoginSuccess", "Account")
                        : model.ReturnUrl
                };
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("User_Message_UserDoesNotExist")
            };
        }

        #endregion

        #endregion

        #region User Profile

        /// <summary>
        /// Get user detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserDetailModel GetUserDetailModel(int id)
        {
            var user = GetById(id);
            if (user != null)
                return new UserDetailModel(user);
            return null;
        }

        /// <summary>
        /// Upload user avatar
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public ResponseModel UploadAvatar(int userId, HttpPostedFileBase avatar)
        {
            var user = GetById(userId);
            if (user != null)
            {
                var extension = avatar.FileName.GetExtension(true);
                var fileName = user.Id.GenerateAvatarFileName(extension);

                var avatarFolder = HttpContext.Current.Server.MapPath(EzCMSContants.AvatarFolder);
                if (!Directory.Exists(avatarFolder))
                {
                    Directory.CreateDirectory(avatarFolder);
                }
                var path = Path.Combine(avatarFolder, fileName);
                var returnPath = string.Format("{0}{1}", EzCMSContants.AvatarFolder, fileName);

                user.AvatarFileName = fileName;
                var response = UpdateUser(user);

                //Refresh current user data
                if (user.Id == WorkContext.CurrentUser.Id)
                {
                    UserSessionModel.SetUserSession(user);
                }

                if (response.Success)
                {
                    avatar.SaveAs(path);
                    response.Data = returnPath;
                }
                return response.SetMessage(response.Success
                    ? T("User_Message_UploadAvatarSuccessfully")
                    : T("User_Message_UploadAvatarFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("User_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Save User Settings

        /// <summary>
        /// Get user grid setting
        /// </summary>
        /// <returns></returns>
        public GridSettingItem GetUserGridSetting(int userId, string name)
        {
            var user = GetById(userId);

            if (user != null)
            {
                var currentSettings = SerializeUtilities.Deserialize<UserSettingModel>(user.Settings);

                if (currentSettings != null)
                {
                    return
                        currentSettings.GridSettingItems.FirstOrDefault(
                            g => !string.IsNullOrEmpty(g.Name) && g.Name.Equals(name));
                }
            }

            return null;
        }

        /// <summary>
        /// Save user grid setting
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public ResponseModel SaveUserGridSetting(int userId, GridSettingItem setting)
        {
            var user = GetById(userId);
            if (user != null)
            {
                var currentSettings = SerializeUtilities.Deserialize<UserSettingModel>(user.Settings);

                if (currentSettings != null)
                {
                    //Remove the current grid setting
                    var gridSetting =
                        currentSettings.GridSettingItems.FirstOrDefault(
                            g => !string.IsNullOrEmpty(g.Name) && g.Name.Equals(setting.Name));
                    if (gridSetting != null)
                    {
                        currentSettings.GridSettingItems.Remove(gridSetting);
                    }

                    //Add new grid setting
                    currentSettings.GridSettingItems.Add(setting);
                }
                else
                {
                    currentSettings = new UserSettingModel
                    {
                        GridSettingItems = new List<GridSettingItem>
                        {
                            setting
                        }
                    };
                }

                user.Settings = SerializeUtilities.Serialize(currentSettings);

                var response = UpdateUser(user);
                if (response.Success)
                    UserSessionModel.SetUserSession(user);
                return response.SetMessage(response.Success
                    ? T("User_Message_ChangeGridSettingSuccessfully")
                    : T("User_Message_ChangeGridSettingFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("User_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Save user settings
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveSetting(ManageSettingModel model)
        {
            var user = GetById(model.UserId);
            if (user != null)
            {
                var currentSettings = SerializeUtilities.Deserialize<UserSettingModel>(user.Settings) ??
                                      new UserSettingModel();

                currentSettings.AdminPageSize = model.AdminPageSize;
                currentSettings.TimeZone = model.TimeZone;
                currentSettings.Culture = model.Culture;

                user.Settings = SerializeUtilities.Serialize(currentSettings);

                var response = UpdateUser(user);
                if (response.Success)
                    UserSessionModel.SetUserSession(user);
                return response.SetMessage(response.Success
                    ? T("User_Message_ChangeGeneralSettingSuccessfully")
                    : T("User_Message_ChangeGeneralSettingFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("User_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Permissions

        /// <summary>
        /// Get user permissions
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<int> GetUserPermissions(int userId)
        {
            var user = GetById(userId);

            if (user == null)
                return new List<int>();

            if (user.IsSystemAdministrator)
            {
                return EnumUtilities.GetAllItems<Permission>().Select(p => (int)p).ToList();
            }

            var permissions = user.UserUserGroups
                .Select(ug => ug.UserGroup.GroupPermissions.Where(gp => gp.HasPermission).Select(gp => gp.PermissionId))
                .Distinct()
                .ToList();
            var userPermissions = new List<int>();

            foreach (var permission in permissions)
            {
                foreach (var i in permission)
                {
                    if (!userPermissions.Contains(i))
                    {
                        userPermissions.Add(i);
                    }
                }
            }
            return userPermissions;
        }

        /// <summary>
        /// Check if user has permissions
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public bool HasPermissions(int userId, params Permission[] permissions)
        {
            // Get current user permissions
            var userPermissions = GetUserPermissions(userId);

            // Get required permissions
            var requiredPermissions = permissions != null ? permissions.Select(p => (int)p).ToList() : new List<int>();

            return userPermissions.Intersect(requiredPermissions).Count() == requiredPermissions.Count();
        }

        /// <summary>
        /// Check if user has one of permissions
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public bool HasOneOfPermissions(int userId, params Permission[] permissions)
        {
            // Get current user permissions
            var modePermissions = GetUserPermissions(userId);

            // Get required permissions
            var requiredPermissions = permissions != null ? permissions.Select(p => (int)p).ToList() : new List<int>();

            return modePermissions.Intersect(requiredPermissions).Any();
        }

        #endregion

        #region Company Admin

        /// <summary>
        /// Search company users
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchCompanyUsers(JqSearchIn si, UserSearchModel model)
        {
            var data = SearchCompanyUsers(model);

            var users = Maps(data);

            return si.Search(users);
        }

        /// <summary>
        /// Export company users
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook ExportCompanyUsers(JqSearchIn si, GridExportMode gridExportMode, UserSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchCompanyUsers(model);

            var users = Maps(data);

            var exportData = si.Export(users, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search company users
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<User> SearchCompanyUsers(UserSearchModel model = null)
        {
            var data = SearchUsers(model);

            if (WorkContext.CurrentUser.IsCompanyAdministrator && !string.IsNullOrEmpty(WorkContext.CurrentUser.Company))
            {
                return
                    data.Where(
                        u => u.Contacts.Any(c => !c.RecordDeleted && c.Company.Equals(WorkContext.CurrentUser.Company)));
            }

            return new List<User>().AsQueryable();
        }

        #endregion

        /// <summary>
        /// Get user details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserDetailModel GetCompanyUserDetailModel(int id)
        {
            var accessableUsers = SearchCompanyUsers();

            var user = accessableUsers.FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                return new UserDetailModel(user);
            }

            return null;
        }

        /// <summary>
        /// Change user status
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        public ResponseModel ChangeCompanyUserStatus(int userId, UserEnums.UserStatus userStatus)
        {
            var accessableUser = SearchCompanyUsers();
            var user = accessableUser.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                user.Status = userStatus;
                var response = UpdateUser(user);
                return response.SetMessage(response.Success
                    ? T("User_Message_ChangeStatusSuccessfully")
                    : T("User_Message_ChangeStatusFailure"));
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("User_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Manage Account Expires

        /// <summary>
        /// Get a list of users that near expiration date
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> GetUsersNearExpirationDate()
        {
            var accountExpiresSetting = _siteSettingService.LoadSetting<AccountExpiredSetting>();
            var compareDate = DateTime.UtcNow.AddDays(accountExpiresSetting.DaysPriorToSuspensionShouldNotify);
            return Fetch(user => user.Status != UserEnums.UserStatus.Disabled
                                 && user.AccountExpiresDate.HasValue
                                 && user.AccountExpiresDate > DateTime.UtcNow
                                 && user.AccountExpiresDate <= compareDate);
        }

        /// <summary>
        /// Get a list of users that need to be deactivated
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> GetUsersNeedDeactive()
        {
            return Fetch(user => user.Status != UserEnums.UserStatus.Disabled
                                 && user.AccountExpiresDate.HasValue
                                 && user.AccountExpiresDate <= DateTime.UtcNow);
        }

        /// <summary>
        /// Deactive expired account
        /// </summary>
        /// <param name="user"></param>
        public ResponseModel DeactiveExpiredAccount(User user)
        {
            user.Status = UserEnums.UserStatus.Disabled;
            return UpdateUser(user);
        }

        /// <summary>
        /// Send deactivation email to user
        /// </summary>
        /// <param name="user"></param>
        public void SendDeactivationEmail(User user)
        {
            var emailLogService = HostContainer.GetInstance<IEmailLogService>();
            var emailTemplateService = HostContainer.GetInstance<IEmailTemplateService>();
            var model = new NotifyAccountDeactivatedEmailModel
            {
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email
            };

            var emailResponse =
                emailTemplateService.ParseEmail(EmailEnums.EmailTemplateType.DeactivateAccountNotification, model);
            var emailLog = new EmailLog
            {
                To = user.Email,
                ToName = user.FullName,
                From = emailResponse.From,
                FromName = emailResponse.FromName,
                CC = emailResponse.CC,
                Bcc = emailResponse.BCC,
                Subject = emailResponse.Subject,
                Body = emailResponse.Body,
                Priority = EmailEnums.EmailPriority.Medium
            };
            emailLogService.CreateEmail(emailLog, true);
        }

        /// <summary>
        /// Extend account expirates date
        /// </summary>
        /// <param name="authorizeCode"></param>
        /// <returns></returns>
        public ExtendExpirationDateResponseModel ExtendAccountExpiratesDate(string authorizeCode)
        {
            var id = 0;
            var email = string.Empty;
            try
            {
                var parts =
                    PasswordUtilities.DecryptString(authorizeCode)
                        .Split(new[] { FrameworkConstants.UniqueLinkSeparator }, StringSplitOptions.None);
                if (parts.Count() == 2)
                {
                    id = parts[0].ToInt();
                    email = parts[1];
                }
            }
            catch (Exception)
            {
                return new ExtendExpirationDateResponseModel
                {
                    ResponseCode = UserEnums.ExtendExpirationDateResponseCode.InvalidUrl
                };
            }

            var user = GetById(id);
            if (user != null && user.Email.Equals(email))
            {
                // User has been disabled
                if (user.Status == UserEnums.UserStatus.Disabled)
                {
                    return new ExtendExpirationDateResponseModel
                    {
                        ResponseCode = UserEnums.ExtendExpirationDateResponseCode.NotActive,
                        UserName = user.FullName,
                        ExpirationDate = user.AccountExpiresDate
                    };
                }

                // Expiration date doesn't need to be extended now
                var accountExpiresSetting = _siteSettingService.LoadSetting<AccountExpiredSetting>();
                if (!user.AccountExpiresDate.HasValue ||
                    user.AccountExpiresDate >
                    DateTime.Now.AddDays(accountExpiresSetting.DaysPriorToSuspensionShouldNotify))
                {
                    return new ExtendExpirationDateResponseModel
                    {
                        ResponseCode = UserEnums.ExtendExpirationDateResponseCode.NoNeedToExtend,
                        UserName = user.FullName,
                        ExpirationDate = user.AccountExpiresDate
                    };
                }

                // Extend expiry date
                user.AccountExpiresDate =
                    user.AccountExpiresDate.Value.AddDays(accountExpiresSetting.NumberOfDaysToKeepAccountAlive);
                var response = UpdateUser(user);

                return new ExtendExpirationDateResponseModel
                {
                    ResponseCode =
                        response.Success
                            ? UserEnums.ExtendExpirationDateResponseCode.Success
                            : UserEnums.ExtendExpirationDateResponseCode.Fail,
                    UserName = user.FullName,
                    ExpirationDate = user.AccountExpiresDate
                };
            }

            return new ExtendExpirationDateResponseModel
            {
                ResponseCode = UserEnums.ExtendExpirationDateResponseCode.UserNotFound
            };
        }

        #endregion
    }
}