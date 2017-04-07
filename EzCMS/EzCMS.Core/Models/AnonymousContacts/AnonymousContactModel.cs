using Ez.Framework.Models;

namespace EzCMS.Core.Models.AnonymousContacts
{
    public class AnonymousContactModel : BaseGridModel
    {
        #region Public Properties

        public int? ContactId { get; set; }

        public string CookieKey { get; set; }

        public string IpAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        #endregion
    }
}