using System;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Subscriptions.SubscriptionTemplates.Base
{
    public class SubscriptionLogItem
    {
        public SubscriptionLogItem()
        {

        }

        public SubscriptionLogItem(SubscriptionLog log)
            : this()
        {
            ChangeLog = log.ChangeLog;
            ChangeDate = log.Created;
        }

        public string ChangeLog { get; set; }

        public DateTime ChangeDate { get; set; }
    }
}
