using Ez.Framework.Configurations;
using Ez.Framework.Core.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace EzCMS.Core.Models.SiteSetup
{
    public class EmailSetupModel
    {
        public EmailSetupModel()
        {
            Timeout = FrameworkConstants.DefaultEmailSendTimeout;
        }

        #region Public Properties

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string DisplayName { get; set; }

        [Required]
        public string Host { get; set; }

        public int Port { get; set; }

        [RequiredIf("UseDefaultCredentials", false)]
        public string UserName { get; set; }

        [RequiredIf("UseDefaultCredentials", false)]
        public string Password { get; set; }

        public bool EnableSsl { get; set; }

        public bool UseDefaultCredentials { get; set; }

        public int Timeout { get; set; }

        #endregion
    }
}