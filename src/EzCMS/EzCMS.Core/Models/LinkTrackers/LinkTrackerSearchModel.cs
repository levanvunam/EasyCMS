using System;

namespace EzCMS.Core.Models.LinkTrackers
{
    public class LinkTrackerSearchModel
    {
        #region Public Properties

        public string Keyword { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        #endregion
    }
}
