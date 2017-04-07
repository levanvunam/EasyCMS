using EzCMS.Entity.Core.Enums;

namespace EzCMS.Core.Models.Notifications.Setup
{
    public class NotificationSetupModel
    {
        public int? Id { get; set; }

        public NotificationEnums.NotificationSetupStep Step { get; set; }

        public NotificationEnums.NotificationModule Module { get; set; }
    }
}
