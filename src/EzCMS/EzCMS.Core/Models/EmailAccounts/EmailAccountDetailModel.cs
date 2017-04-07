using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.EmailAccounts
{
    public class EmailAccountDetailModel
    {
        public EmailAccountDetailModel()
        {
        }

        public EmailAccountDetailModel(EmailAccount emailAccount)
            : this()
        {
            Id = emailAccount.Id;

            EmailAccount = new EmailAccountManageModel(emailAccount);

            Created = emailAccount.Created;
            CreatedBy = emailAccount.CreatedBy;
            LastUpdate = emailAccount.LastUpdate;
            LastUpdateBy = emailAccount.LastUpdateBy;

        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("EmailAccount_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("EmailAccount_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("EmailAccount_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("EmailAccount_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public EmailAccountManageModel EmailAccount { get; set; }

        #endregion
    }
}
