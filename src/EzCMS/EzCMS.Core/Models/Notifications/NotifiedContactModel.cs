using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Notifications
{
    public class NotifiedContactModel
    {
        #region Constructors

        public NotifiedContactModel()
        {
        }

        public NotifiedContactModel(Contact contact)
            : this()
        {
            FirstName = contact.FirstName;
            LastName = contact.LastName;
            Email = contact.Email;
            PhoneNumber = contact.PreferredPhoneNumber;
        }

        #endregion

        #region Public Properties

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        #endregion
    }
}
