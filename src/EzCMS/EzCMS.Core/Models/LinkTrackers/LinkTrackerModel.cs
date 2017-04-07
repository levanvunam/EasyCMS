using Ez.Framework.Models;

namespace EzCMS.Core.Models.LinkTrackers
{
    public class LinkTrackerModel : BaseGridModel
    {
        #region Public Properties

        public string Name { get; set; }

        public string TrackerLink { get; set; }

        public bool IsAllowMultipleClick { get; set; }

        public string RedirectUrl { get; set; }

        public int? PageId { get; set; }

        public string PageTitle { get; set; }

        public int ClickCount { get; set; }

        #endregion
    }
}
