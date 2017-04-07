using System.ComponentModel;

namespace EzCMS.Entity.Core.Enums
{
    public class SubscriptionEnums
    {
        public enum SubscriptionModule
        {
            [Description("Page Subscription")]
            Page = 1
        }

        public enum SubscriptionType
        {
            Midnight = 1,
            Instantly = 2
        }

        public enum SubscriptionAction
        {
            View = 1,
            Remove = 2
        }
    }
}
