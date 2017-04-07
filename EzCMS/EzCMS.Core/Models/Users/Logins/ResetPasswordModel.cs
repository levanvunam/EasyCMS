using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.Users;
using Ez.Framework.IoC;
using EzCMS.Core.Framework.Attributes.Validation;
using EzCMS.Core.Framework.Enums;

namespace EzCMS.Core.Models.Users.Logins
{
    public class ResetPasswordModel : LoginSetupModel, IValidatableObject
    {
        public ResetPasswordModel()
            : base(LoginEnums.LoginTemplateConfiguration.ResetPassword)
        {
            
        }

        #region Public Properties
        
        public int UserId { get; set; }

        public string SecurityCode { get; set; }

        public bool IsValidCode { get; set; }

        [Required]
        [EzCMSPasswordComplexValidation]
        public string Password { get; set; }

        [Required]
        [DisplayName(@"Confirm Password")]
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
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            var user = userService.GetById(UserId);

            if (user == null || string.IsNullOrWhiteSpace(user.ResetPasswordCode) || !user.ResetPasswordCode.Equals(SecurityCode, StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationResult(localizedResourceService.T("User_Message_InvalidSecurityCode"));
            }
        }
    }
}
