using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.Users;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Users.Logins
{
    public class RegisterModel : LoginSetupModel, IValidatableObject
    {
        public RegisterModel()
            : base(LoginEnums.LoginTemplateConfiguration.Register)
        {

        }

        #region Public Properties

        [UsernameValidation]
        [LocalizedDisplayName("User_Field_Username")]
        public string Username { get; set; }

        [Required]
        [EmailValidation]
        [LocalizedDisplayName("User_Field_Email")]
        public string Email { get; set; }

        [Required]
        [LocalizedDisplayName("User_Field_FirstName")]
        public string FirstName { get; set; }

        [Required]
        [LocalizedDisplayName("User_Field_LastName")]
        public string LastName { get; set; }

        [Required]
        [LocalizedDisplayName("User_Field_Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [LocalizedDisplayName("User_Field_ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [LocalizedDisplayName("User_Field_UserAgreement")]
        public bool UserAgreement { get; set; }

        public string ReturnUrl { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var userService = HostContainer.GetInstance<IUserService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();

            var username = string.IsNullOrEmpty(Username) ? Email : Username;
            if (userService.IsUsernameExisted(null, username))
            {
                yield return new ValidationResult(localizedResourceService.T("User_Message_ExistingUsername"), new[] { "Username" });
            }
            if (!UserAgreement)
            {
                yield return new ValidationResult(localizedResourceService.T("User_Message_UserAgreementNotAccepted"), new[] { "UserAgreement" });
            }
        }
    }
}
