using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Core.Enums;
using EzCMS.Core.Framework.Attributes.Validation;
using EzCMS.Core.Framework.Enums;

namespace EzCMS.Core.Models.Users.Logins
{
    public class ChangePasswordAfterLoginModel : LoginSetupModel
    {
        #region Constructors

        public ChangePasswordAfterLoginModel()
            : base(LoginEnums.LoginTemplateConfiguration.ChangePasswordAfterLogin)
        {
            
        }

        #endregion

        #region Public Properties

        public int UserId { get; set; }

        [Required]
        [EzCMSPasswordComplexValidation]
        public string Password { get; set; }

        [Required]
        [DisplayName(@"Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }

        #endregion
    }
}
