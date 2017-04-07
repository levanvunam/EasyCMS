using AutoMapper;
using Ez.Framework.Core.Context;
using Ez.Framework.Core.Entity.Models;
using Ez.Framework.Core.Extensions;
using Ez.Framework.Core.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Models.Users.Settings;
using EzCMS.Core.Services.Companies;
using EzCMS.Core.Services.CompanyTypes;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Core.Services.Users;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EzCMS.Core.Models.Users
{
    public class UserSessionModel : BaseModel
    {
        #region Constructors

        public UserSessionModel()
        {
            GroupIds = new List<int>();
            CompanyIds = new List<int>();
            CompanyTypeIds = new List<int>();
        }

        #endregion

        #region Public Properties

        public string Identity
        {
            get { return string.IsNullOrEmpty(Username) ? Email : Username; }
        }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public string AvatarPath
        {
            get
            {
                if (string.IsNullOrEmpty(AvatarFileName) || !File.Exists(HttpContext.Current.Server.MapPath(EzCMSContants.AvatarFolder + AvatarFileName)))
                {
                    return EzCMSContants.NoAvatar;
                }
                return EzCMSContants.AvatarFolder + AvatarFileName;
            }
        }

        public string CurrentAvatarFileName
        {
            get
            {
                if (string.IsNullOrEmpty(AvatarFileName)
                    || !File.Exists(HttpContext.Current.Server.MapPath(EzCMSContants.AvatarFolder + AvatarFileName)))
                {
                    return EzCMSContants.NoAvatar;
                }
                return AvatarFileName;
            }
        }

        public double LastLoginHours
        {
            get
            {
                if (LastLogin.HasValue)
                    return (DateTime.UtcNow - LastLogin.Value).TotalHours;
                return 0;
            }
        }

        public int? Age
        {
            get
            {
                if (BirthDay.HasValue)
                {
                    return (int)(DateTime.UtcNow - BirthDay.Value).TotalDays / 365;
                }
                return null;
            }
        }

        public UserSettingModel UserSettingModel
        {
            get
            {
                var settingService = HostContainer.GetInstance<ISiteSettingService>();
                var setting = string.IsNullOrEmpty(Settings)
                    ? new UserSettingModel()
                    : SerializeUtilities.Deserialize<UserSettingModel>(Settings);
                if (setting.AdminPageSize <= 0)
                {
                    setting.AdminPageSize = settingService.GetSetting<int>(SettingNames.DefaultGridPageSize);
                }
                return setting;
            }
        }

        #region Contact Information

        public List<int> GroupIds { get; set; }

        public List<int> CompanyIds { get; set; }

        public string Company { get; set; }

        public string CompanyType { get; set; }

        public List<int> CompanyTypeIds { get; set; }

        public bool IsCompanyAdministrator { get; set; }

        #endregion

        #region Additional Session

        public dynamic Session { get; set; }

        #endregion

        #region Properties

        public string Username { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public DateTime? BirthDay { get; set; }

        public int? Gender { get; set; }

        public string About { get; set; }

        public string AvatarFileName { get; set; }

        public string Address { get; set; }

        public UserEnums.UserStatus Status { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime? LastTimeGettingUpdate { get; set; }

        #region Social

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        public string LinkedIn { get; set; }

        #endregion

        public bool IsRemoteAccount { get; set; }

        public bool IsSystemAdministrator { get; set; }

        public string Settings { get; set; }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Set user session after login
        /// </summary>
        /// <param name="user"></param>
        /// <param name="lastGettingUpdates"></param>
        /// <param name="session"></param>
        public static void SetUserSession(User user, DateTime? lastGettingUpdates = null, dynamic session = null)
        {
            var companyService = HostContainer.GetInstance<ICompanyService>();
            var companyTypeService = HostContainer.GetInstance<ICompanyTypeService>();
            Mapper.CreateMap<User, UserSessionModel>().IgnoreAllNonExisting();
            var userSessionModel = Mapper.Map<UserSessionModel>(user);

            // Set the groupIds
            userSessionModel.GroupIds = user.UserUserGroups != null ? user.UserUserGroups.Select(g => g.UserGroupId).ToList() : new List<int>();

            // Get the last time user login and get updates
            userSessionModel.LastTimeGettingUpdate = lastGettingUpdates ?? user.LastLogin;

            // Save session from external service to current user
            if (session != null)
            {
                userSessionModel.Session = session;
            }

            // Setup the contact information
            if (user.Contacts.Any())
            {
                //Check if user is company admin
                userSessionModel.IsCompanyAdministrator = user.Contacts.Any(c => !c.RecordDeleted && c.IsCompanyAdministrator);

                var companyNames = user.Contacts.Where(c => !string.IsNullOrEmpty(c.Company)).Select(c => c.Company).ToList();

                // Get the first company as user company
                if (companyNames.Any()) userSessionModel.Company = companyNames.First();

                //Generate company ids
                userSessionModel.CompanyIds =
                    companyService.Fetch(c => companyNames.Contains(c.Name)).Select(c => c.Id).ToList();

                var companyTypeIds =
                    companyService.Fetch(c => companyNames.Contains(c.Name) && c.CompanyTypeId.HasValue)
                        .Select(c => c.CompanyTypeId.Value)
                        .ToList();

                //Generate company type ids
                userSessionModel.CompanyTypeIds = companyTypeIds;

                if (companyTypeIds.Any())
                {
                    userSessionModel.CompanyType =
                        companyTypeService.FetchFirst(c => companyTypeIds.Contains(c.Id)).Name;
                }
            }

            WorkContext.CurrentUser = userSessionModel;
            EzWorkContext.CurrentUserId = userSessionModel.Id;
            WorkContext.CurrentContact = new ContactCookieModel(user);

            var userSettings = SerializeUtilities.Deserialize<UserSettingModel>(user.Settings);
            if (userSettings != null)
            {
                WorkContext.CurrentCulture = userSettings.Culture;
            }
        }

        /// <summary>
        /// Check if user has all defined permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public bool HasPermissions(params Permission[] permissions)
        {
            var userService = HostContainer.GetInstance<IUserService>();

            return userService.HasPermissions(Id, permissions);
        }

        /// <summary>
        /// Check if user has one of permission
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public bool HasOneOfPermissions(params Permission[] permissions)
        {
            var userService = HostContainer.GetInstance<IUserService>();

            return userService.HasOneOfPermissions(Id, permissions);
        }

        #endregion
    }
}