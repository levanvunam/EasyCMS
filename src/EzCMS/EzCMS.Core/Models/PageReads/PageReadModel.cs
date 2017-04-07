using Ez.Framework.Models;

namespace EzCMS.Core.Models.PageReads
{
    public class PageReadModel : BaseGridModel
    {
        #region Public Properties

        public int PageId { get; set; }

        public string Title { get; set; }

        public string FriendlyUrl { get; set; }

        public int? ContactId { get; set; }

        public string CookieKey { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        #endregion
    }
}
