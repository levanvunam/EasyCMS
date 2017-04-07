using System.Collections.Generic;
using EzCMS.Core.Models.Pages;
using EzCMS.Core.Models.Subscriptions.SubscriptionTemplates.Base;

namespace EzCMS.Core.Models.Subscriptions.Widgets.ContentUpdates
{
    public class PageSubscriptionLogModel
    {
        public PageSubscriptionLogModel()
        {
            Logs = new List<SubscriptionLogItem>();
        }

        #region Public Properties

        public PageModel Page { get; set; }

        public List<SubscriptionLogItem> Logs { get; set; }

        #endregion
    }
}
