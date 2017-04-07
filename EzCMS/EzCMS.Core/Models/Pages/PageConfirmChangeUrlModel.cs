using System.Collections.Generic;

namespace EzCMS.Core.Models.Pages
{
    public class PageConfirmChangeUrlModel
    {
        #region Public Properties

        public string NewUrl { get; set; }

        public string OldUrl { get; set; }

        public List<PageConfirmChangeUrlItemModel> ReferencedPages { get; set; }

        #endregion
    }

    public class PageConfirmChangeUrlItemModel
    {
        #region Public Properties

        public int Id { get; set; }

        public string Title { get; set; }

        public string FriendlyUrl { get; set; }

        #endregion
    }
}
