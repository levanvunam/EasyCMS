using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Validation;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Models.Users.Settings;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.UserGroups;
using EzCMS.Core.Services.Users;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Users
{
    public class UserManageModel : IValidatableObject
    {
        #region Constructors

        private readonly IUserGroupService _userGroupService;
        public UserManageModel()
        {
            _userGroupService = HostContainer.GetInstance<IUserGroupService>();

            UserGroups = _userGroupService.GetUserGroups();
            Genders = EnumUtilities.GetAllItems<UserEnums.Gender>();
            StatusList = EnumUtilities.GenerateSelectListItems<UserEnums.UserStatus>();
            ManageSettingModel = new ManageSettingModel();
        }

        public UserManageModel(User user)
            : this()
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Email = user.Email;
            IsSystemAdministrator = user.IsSystemAdministrator;
            Status = user.Status;
            Phone = user.Phone;
            Gender = user.Gender;
            About = user.About;
            Address = user.Address;
            Facebook = user.Facebook;
            Twitter = user.Twitter;
            LinkedIn = user.LinkedIn;
            AvatarFileName = user.AvatarFileName;
            DateOfBirth = user.DateOfBirth;
            UserGroupIds = user.UserUserGroups.Select(g => g.UserGroupId).ToList();
            UserGroups = _userGroupService.GetUserGroups(UserGroupIds);
            IsRemoteAccount = user.IsRemoteAccount;
            ChangePasswordAfterLogin = user.ChangePasswordAfterLogin;

            RecordOrder = user.RecordOrder;
            RecordDeleted = user.RecordDeleted;

            Created = user.Created;
            CreatedBy = user.CreatedBy;
            LastUpdate = user.LastUpdate;
            LastUpdateBy = user.LastUpdateBy;

            ManageSettingModel = new ManageSettingModel(user);
        }

        public UserManageModel(SimpleUserManageModel user)
            : this()
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Email = user.Email;
            Status = user.Status;
            Password = user.Password;
            IsRemoteAccount = user.IsRemoteAccount;
            IsSystemAdministrator = user.IsSystemAdministrator;
            Phone = user.Phone;
            Address = user.Address;
            UserGroupIds = user.UserGroupIds;
            UserGroups = _userGroupService.GetUserGroups(UserGroupIds);

            if (user.Id != null) ManageSettingModel = new ManageSettingModel(user.Id.Value);
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [UsernameValidation]
        [StringLength(255)]
        [LocalizedDisplayName("User_Field_Username")]
        public string Username { get; set; }

        [Required]
        [EmailValidation]
        [StringLength(255)]
        [LocalizedDisplayName("User_Field_Email")]
        public string Email { get; set; }

        [RequiredIf("Id", null)]
        [StringLength(512)]
        [EzCMSPasswordComplexValidation]
        [LocalizedDisplayName("User_Field_Password")]
        public string Password { get; set; }

        [StringLength(512)]
        [LocalizedDisplayName("User_Field_ConfirmPassword")]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }

        [LocalizedDisplayName("User_Field_FullName")]
        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        [Required]
        [StringLength(512)]
        [LocalizedDisplayName("User_Field_FirstName")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(512)]
        [LocalizedDisplayName("User_Field_LastName")]
        public string LastName { get; set; }

        [LocalizedDisplayName("User_Field_IsSystemAdministrator")]
        public bool IsSystemAdministrator { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("User_Field_Phone")]
        public string Phone { get; set; }

        [LocalizedDisplayName("User_Field_Gender")]
        public UserEnums.Gender Gender { get; set; }

        public IEnumerable<UserEnums.Gender> Genders { get; set; }

        [LocalizedDisplayName("User_Field_About")]
        public string About { get; set; }

        [StringLength(1024)]
        [LocalizedDisplayName("User_Field_Address")]
        public string Address { get; set; }

        [StringLength(512)]
        [LocalizedDisplayName("User_Field_AvatarFileName")]
        public string AvatarFileName { get; set; }

        [LocalizedDisplayName("User_Field_IsRemoteAccount")]
        public bool IsRemoteAccount { get; set; }

        [LocalizedDisplayName("User_Field_ChangePasswordAfterLogin")]
        public bool ChangePasswordAfterLogin { get; set; }

        [LocalizedDisplayName("User_Field_UserGroupIds")]
        public List<int> UserGroupIds { get; set; }

        public IEnumerable<SelectListItem> UserGroups { get; set; }

        [StringLength(512)]
        [LocalizedDisplayName("User_Field_Facebook")]
        public string Facebook { get; set; }

        [StringLength(512)]
        [LocalizedDisplayName("User_Field_Twitter")]
        public string Twitter { get; set; }

        [StringLength(512)]
        [LocalizedDisplayName("User_Field_LinkedIn")]
        public string LinkedIn { get; set; }

        [LocalizedDisplayName("User_Field_DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [LocalizedDisplayName("User_Field_Status")]
        public UserEnums.UserStatus Status { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }

        [LocalizedDisplayName("User_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        [LocalizedDisplayName("User_Field_RecordDeleted")]
        public bool RecordDeleted { get; set; }

        [LocalizedDisplayName("User_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("User_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("User_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("User_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

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

        public ManageSettingModel ManageSettingModel { get; set; }

        public int? Age
        {
            get
            {
                if (DateOfBirth.HasValue)
                {
                    return (int)(DateTime.UtcNow - DateOfBirth.Value).TotalDays / 365;
                }
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var userService = HostContainer.GetInstance<IUserService>();

            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();

            if (Id <= 0)
            {
                if (string.IsNullOrWhiteSpace(Email))
                    yield return new ValidationResult(localizedResourceService.T("User_Message_EmailRequired"), new[] { "Email" });

                if (string.IsNullOrWhiteSpace(Password))
                    yield return new ValidationResult(localizedResourceService.T("User_Message_PasswordRequired"), new[] { "Password" });
            }

            var username = string.IsNullOrEmpty(Username) ? Email : Username;
            if (userService.IsUsernameExisted(Id, username))
            {
                if (string.IsNullOrEmpty(Username))
                {
                    yield return new ValidationResult(localizedResourceService.T("User_Message_ExistingEmail"), new[] { "Email" });
                }
                else
                {
                    yield return new ValidationResult(localizedResourceService.T("User_Message_ExistingUsername"), new[] { "Username" });
                }
            }
        }
    }
}
