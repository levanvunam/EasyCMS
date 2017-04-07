using Ez.Framework.Models;

namespace EzCMS.Core.Models.NewsReads
{
    public class NewsReadModel : BaseGridModel
    {
        #region Public Properties

        public int NewsId { get; set; }

        public string Title { get; set; }

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
