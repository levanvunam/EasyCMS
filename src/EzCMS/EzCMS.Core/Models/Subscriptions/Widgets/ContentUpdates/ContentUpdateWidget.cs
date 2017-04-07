using System.Collections.Generic;
using EzCMS.Core.Models.Subscriptions.SubscriptionTemplates;

namespace EzCMS.Core.Models.Subscriptions.Widgets.ContentUpdates
{
    public class ContentUpdateWidget
    {
        public ContentUpdateWidget()
        {
            PageLogs = new List<PageSubscriptionLogModel>();
        }

        #region Public Properties

        public int TotalChanges { get; set; }

        public int TotalLogs { get; set; }

        public List<PageSubscriptionLogModel> PageLogs { get; set; }

        #endregion
    }
}
