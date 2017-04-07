using Ez.Framework.Configurations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using EzCMS.Entity.Entities.Models;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.EmailAccounts
{
    public class EmailAccountManageModel
    {
        #region Contructors

        public EmailAccountManageModel()
        {
            Timeout = FrameworkConstants.DefaultEmailSendTimeout;
            IsDefault = false;
        }

        public EmailAccountManageModel(EmailAccount emailAccount)
        {
            Id = emailAccount.Id;
            Email = emailAccount.Email;
            DisplayName = emailAccount.DisplayName;
            Host = emailAccount.Host;
            Port = emailAccount.Port;
            Username = emailAccount.Username;
            Password = emailAccount.Password;
            EnableSsl = emailAccount.EnableSsl;
            UseDefaultCredentials = emailAccount.UseDefaultCredentials;
            IsDefault = emailAccount.IsDefault;
            Timeout = emailAccount.TimeOut;
        }
        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [EmailValidation]
        [LocalizedDisplayName("EmailAccount_Field_Email")]
        public string Email { get; set; }

        [Required]
        [LocalizedDisplayName("EmailAccount_Field_DisplayName")]
        public string DisplayName { get; set; }

        [Required]
        [LocalizedDisplayName("EmailAccount_Field_Host")]
        public string Host { get; set; }

        [Required]
        [LocalizedDisplayName("EmailAccount_Field_Port")]
        public int Port { get; set; }

        [RequiredIf("UseDefaultCredentials", false)]
        [LocalizedDisplayName("EmailAccount_Field_Username")]
        public string Username { get; set; }

        [RequiredIf("UseDefaultCredentials", false)]
        [LocalizedDisplayName("EmailAccount_Field_Password")]
        public string Password { get; set; }

        [LocalizedDisplayName("EmailAccount_Field_EnableSSL")]
        public bool EnableSsl { get; set; }

        [LocalizedDisplayName("EmailAccount_Field_UseDefaultCredentials")]
        public bool UseDefaultCredentials { get; set; }

        [Required]
        [LocalizedDisplayName("EmailAccount_Field_Timeout")]
        public int Timeout { get; set; }

        [LocalizedDisplayName("EmailAccount_Field_IsDefault")]
        public bool IsDefault { get; set; }
        #endregion
    }
}
