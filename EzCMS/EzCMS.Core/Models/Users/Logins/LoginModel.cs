using EzCMS.Core.Framework.Enums;
using System.ComponentModel.DataAnnotations;

namespace EzCMS.Core.Models.Users.Logins
{
    public class LoginModel : LoginSetupModel
    {
        public LoginModel()
            : base(LoginEnums.LoginTemplateConfiguration.Login)
        {

        }

        #region Public Properties

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        #endregion
    }
}