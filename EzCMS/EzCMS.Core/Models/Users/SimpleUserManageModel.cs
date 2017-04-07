using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using Ez.Framework.Core.Attributes.Validation;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.UserGroups;
using EzCMS.Core.Services.Users;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Framework.Attributes.Validation;

namespace EzCMS.Core.Models.Users
{
    public class SimpleUserManageModel : IValidatableObject
    {
        private readonly IUserGroupService _userGroupService;
        public SimpleUserManageModel()
        {
            _userGroupService = HostContainer.GetInstance<IUserGroupService>();

            UserGroups = _userGroupService.GetUserGroups();
            UserGroupIds = new List<int>();

            StatusList = EnumUtilities.GenerateSelectListItems<UserEnums.UserStatus>();
        }

        public SimpleUserManageModel(User user)
            : this()
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Email = user.Email;
            Password = user.Password;
            ConfirmPassword = user.Password;
            Status = user.Status;
            Phone = user.Phone;
            Address = user.Address;
            UserGroupIds = user.UserUserGroups.Select(g => g.UserGroupId).ToList();
            UserGroups = _userGroupService.GetUserGroups(UserGroupIds);
        }

        public SimpleUserManageModel(Contact contact)
            : this()
        {
            FirstName = contact.FirstName;
            LastName = contact.LastName;
            Email = contact.Email;
            Status = UserEnums.UserStatus.Active;
            Phone = contact.PreferredPhoneNumber;
            Address = contact.AddressLine1;
        }

        #region Public Properties

        public int? Id { get; set; }

        [StringLength(255)]
        [UsernameValidation]
        [LocalizedDisplayName("User_Field_Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        [EmailValidation]
        [LocalizedDisplayName("User_Field_Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(512)]
        [EzCMSPasswordComplexValidation]
        [LocalizedDisplayName("User_Field_Password")]
        public string Password { get; set; }

        [Required]
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

        [StringLength(50)]
        [LocalizedDisplayName("User_Field_Phone")]
        public string Phone { get; set; }

        [StringLength(1024)]
        [LocalizedDisplayName("User_Field_Address")]
        public string Address { get; set; }

        [LocalizedDisplayName("User_Field_IsRemoteAccount")]
        public bool IsRemoteAccount { get; set; }

        [LocalizedDisplayName("User_Field_IsSystemAdministrator")]
        public bool IsSystemAdministrator { get; set; }

        [LocalizedDisplayName("User_Field_UserGroupIds")]
        public List<int> UserGroupIds { get; set; }

        public IEnumerable<SelectListItem> UserGroups { get; set; }

        [LocalizedDisplayName("User_Field_Status")]
        public UserEnums.UserStatus Status { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }

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
