using EzCMS.Core.Models.Pages;
using EzCMS.Core.Models.Subscriptions.SubscriptionTemplates.Base;

namespace EzCMS.Core.Models.Subscriptions.SubscriptionTemplates
{
    public class PageSubscriptionEmailModel : SubscriptionEmailModel
    {
        public PageModel Page { get; set; }
    }
}
