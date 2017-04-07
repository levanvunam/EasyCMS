using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.NotificationTemplates;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Notifications.Setup
{
    public class NotificationTemplateSetupModel : NotificationSetupModel
    {
        #region Constructors

        private readonly INotificationTemplateService _notificationTemplateService;
        public NotificationTemplateSetupModel()
        {
            _notificationTemplateService = HostContainer.GetInstance<INotificationTemplateService>();

            Step = NotificationEnums.NotificationSetupStep.SetupNotificationTemplate;
        }

        public NotificationTemplateSetupModel(NotificationEnums.NotificationModule module)
            : this()
        {
            NotificationTemplates = _notificationTemplateService.GetNotificationTemplates(module);
        }

        public NotificationTemplateSetupModel(Notification notification)
            : this(notification.Module)
        {
            Id = notification.Id;

            if (string.IsNullOrEmpty(notification.NotificationBody))
            {
                NotificationTemplates = _notificationTemplateService.GetNotificationTemplates(notification.Module, true);
                var defaultTemplate = _notificationTemplateService.GetDefaultNotificationTemplate(notification.Module);

                NotificationSubject = defaultTemplate.Subject;
                NotificationBody = defaultTemplate.Body;
            }
            else
            {
                NotificationSubject = notification.NotificationSubject;
                NotificationBody = notification.NotificationBody;
            }
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("Notification_Field_NotificationSubject")]
        public string NotificationSubject { get; set; }

        [LocalizedDisplayName("Notification_Field_NotificationBody")]
        public string NotificationBody { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> NotificationTemplates { get; set; }

        #endregion
    }
}
