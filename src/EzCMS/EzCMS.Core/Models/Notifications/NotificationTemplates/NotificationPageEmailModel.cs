using EzCMS.Core.Models.Pages;
using EzCMS.Core.Models.Notifications.NotificationTemplates.Base;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Notifications.NotificationTemplates
{
    public class NotificationPageEmailModel : NotificationEmailModel
    {
        public PageModel Page { get; set; }

        public string ViewUrl
        {
            get
            {
                return Page.FriendlyUrl.ToPageFriendlyUrl(Page.IsHomePage).ToAbsoluteUrl();
            }
        }
    }
}
