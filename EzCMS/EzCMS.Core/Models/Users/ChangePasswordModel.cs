using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.Users;
using EzCMS.Core.Services.Users.RemoteUsers;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Attributes.Validation;

namespace EzCMS.Core.Models.Users
{
    public class ChangePasswordModel : IValidatableObject
    {
        #region Public Properties

        public int UserId { get; set; }

        [Required]
        [LocalizedDisplayName("Account_Field_OldPassword")]
        public string OldPassword { get; set; }

        [Required]
        [LocalizedDisplayName("Account_Field_NewPassword")]
        [EzCMSPasswordComplexValidation]
        public string Password { get; set; }

        [Required]
        [LocalizedDisplayName("Account_Field_ConfirmNewPassword")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var userService = HostContainer.GetInstance<IUserService>();
            var remoteService = HostContainer.GetInstance<IRemoteUserService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            var user = userService.GetById(UserId);
            if (!user.IsRemoteAccount)
            {
                var hashPass = user.Password;
                var hashPassCompare = PasswordUtilities.CreateHashPassword(OldPassword, user.PasswordSalt);
                if (!hashPass.Equals(hashPassCompare))
                {
                    yield return new ValidationResult(localizedResourceService.T("User_Message_InvalidOldPassword"));
                }
            }
            else
            {
                var username = string.IsNullOrEmpty(user.Username) ? user.Email : user.Username;
                if (!remoteService.ValidateUsernamePassword(username, OldPassword))
                {
                    yield return new ValidationResult(localizedResourceService.T("User_Message_InvalidOldPassword"));
                }
            }
        }
    }
}
