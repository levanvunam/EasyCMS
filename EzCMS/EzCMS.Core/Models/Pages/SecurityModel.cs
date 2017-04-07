namespace EzCMS.Core.Models.Pages
{
    public class SecurityModel
    {
        #region Public Properties

        public int Id { get; set; }

        public int PageId { get; set; }

        public string PageTitle { get; set; }

        public bool CanView { get; set; }

        public bool CanEdit { get; set; }

        #endregion
    }
}
