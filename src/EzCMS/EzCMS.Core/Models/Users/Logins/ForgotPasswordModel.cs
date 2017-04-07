using EzCMS.Core.Framework.Enums;
using System.ComponentModel.DataAnnotations;

namespace EzCMS.Core.Models.Users.Logins
{
    public class ForgotPasswordModel : LoginSetupModel
    {
        public ForgotPasswordModel()
            : base(LoginEnums.LoginTemplateConfiguration.ForgotPassword)
        {

        }

        #region Public Properties

        [Required]
        public string Username { get; set; }

        #endregion
    }
}
