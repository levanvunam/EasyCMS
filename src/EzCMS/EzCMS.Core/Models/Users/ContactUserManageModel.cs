using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Users
{
    public class ContactUserManageModel : SimpleUserManageModel
    {
        public ContactUserManageModel()
        {
        }

        public ContactUserManageModel(Contact contact) : base(contact)
        {
            ContactId = contact.Id;
            ContactEmail = contact.Email;
        }

        #region Public Properties

        public int ContactId { get; set; }

        public string ContactEmail { get; set; }

        #endregion
    }
}
