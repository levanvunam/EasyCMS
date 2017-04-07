using System.ComponentModel;

namespace EzCMS.Entity.Core.Enums
{
    public class ContactEnums
    {
        public enum DontSendMarketing
        {
            [Description("All")]
            All = 0,
            [Description("Send")]
            Send = 1,
            [Description("Dont Send")]
            DontSend = 2
        }

        public enum NewsletterSubscribed
        {
            [Description("All")]
            All = 0,
            [Description("Subscribed")]
            Subscribe = 1,
            [Description("Unsubscribed")]
            Unsubscribe = 2
        }

        public enum InterestInOwning
        {
            [Description("All")]
            All = 0,
            [Description("Is Interest In Owning")]
            IsInterest = 1,
            [Description("Is Not Interest In Owning")]
            IsNotInterest = 2
        }
    }
}
