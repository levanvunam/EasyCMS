using System.Collections.Generic;

namespace EzCMS.Core.Models.Subscriptions.Subscribers
{
    public class SubscriberModel
    {
        public string Email { get; set; }

        public List<SubscribeModule> SubscribeModules { get; set; }
    }
}
