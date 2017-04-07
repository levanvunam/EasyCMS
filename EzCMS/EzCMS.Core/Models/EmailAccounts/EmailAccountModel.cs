using Ez.Framework.Models;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.EmailAccounts
{
    public class EmailAccountModel : BaseGridModel
    {
        public EmailAccountModel()
        {
            
        }

        public EmailAccountModel(EmailAccount emailAccount)
        {
            Email = emailAccount.Email;
            DisplayName = emailAccount.DisplayName;
            IsDefault = emailAccount.IsDefault;

            Created = emailAccount.Created;
            CreatedBy = emailAccount.CreatedBy;
            LastUpdate = emailAccount.LastUpdate;
            LastUpdateBy = emailAccount.LastUpdateBy;
        }
        #region Public Properties

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public bool IsDefault { get; set; }

        #endregion
    }
}
