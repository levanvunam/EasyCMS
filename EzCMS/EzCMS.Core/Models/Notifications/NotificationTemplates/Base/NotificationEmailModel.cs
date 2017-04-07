using EzCMS.Core.Models.Contacts;

namespace EzCMS.Core.Models.Notifications.NotificationTemplates.Base
{
    public class NotificationEmailModel
    {
        public int NotificationId { get; set; }

        public ContactModel Contact { get; set; }
    }
}
