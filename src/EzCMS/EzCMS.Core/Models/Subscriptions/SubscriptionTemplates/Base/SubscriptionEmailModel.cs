using System.Collections.Generic;
using System.Web;
using EzCMS.Entity.Core.Enums;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Subscriptions.SubscriptionTemplates.Base
{
    public class SubscriptionEmailModel
    {
        public int SubscriptionId { get; set; }

        public string Parameters { get; set; }

        public string ViewUrl
        {
            get
            {
                return UrlUtilities.GenerateUrl(HttpContext.Current.Request.RequestContext, "Subscription", "SiteApi",
                    new { subscriptionAction = SubscriptionEnums.SubscriptionAction.View, subscriptionId = SubscriptionId }, true);
            }
        }

        public string RemoveUrl
        {
            get
            {
                return UrlUtilities.GenerateUrl(HttpContext.Current.Request.RequestContext, "Subscription", "SiteApi",
                    new { subscriptionAction = SubscriptionEnums.SubscriptionAction.Remove, subscriptionId = SubscriptionId }, true);
            }
        }

        public List<SubscriptionLogItem> Logs { get; set; }
    }
}
