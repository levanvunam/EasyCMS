using System.ComponentModel;

namespace EzCMS.Entity.Core.Enums
{
    public class NotificationEnums
    {
        public enum NotificationModule
        {
            [Description("Page Notification")]
            Page = 1
        }

        public enum NotificationSetupStep
        {
            [Description("Setup Contact Group")]
            SetupContactGroup = 1,

            [Description("Setup Notification WidgetTemplate")]
            SetupNotificationTemplate = 2,

            [Description("Saving Configuration")]
            SavingConfiguration = 3
        }
    }
}
