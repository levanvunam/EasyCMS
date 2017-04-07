using EzCMS.Entity.Core.Enums;

namespace EzCMS.Core.Models.Subscriptions.Subscribers
{
    public class SubscribeModule
    {
        #region Public Properties

        public int SubscriptionId { get; set; }

        public SubscriptionEnums.SubscriptionModule Module { get; set; }

        public string Parameters { get; set; }

        #endregion
    }
}
