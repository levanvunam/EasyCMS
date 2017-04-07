using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.ContactGroups;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.NotificationTemplates;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Notifications.Setup.NotificationSetup
{
    public class NotificationConfigurationSetupModel : NotificationSetupModel, IValidatableObject
    {
        #region Constructors

        public NotificationConfigurationSetupModel()
        {
            Step = NotificationEnums.NotificationSetupStep.SavingConfiguration;
        }

        public NotificationConfigurationSetupModel(Notification notification)
            : this()
        {
            Id = notification.Id;
            SendTime = notification.SendTime;
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("Notification_Field_NotificationTemplateName")]
        public string NotificationTemplateName { get; set; }

        [LocalizedDisplayName("Notification_Field_ContactGroupName")]
        public string ContactGroupName { get; set; }

        [LocalizedDisplayName("Notification_Field_SendTime")]
        public DateTime? SendTime { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var contactGroupService = HostContainer.GetInstance<IContactGroupService>();
            var notificationTemplateService = HostContainer.GetInstance<INotificationTemplateService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (contactGroupService.IsNameExisted(Id, ContactGroupName))
            {
                yield return new ValidationResult(localizedResourceService.T("ContactGroup_Message_ExistingName"), new[] { "ContactGroupName" });
            }

            if (notificationTemplateService.IsNameExisted(Id, NotificationTemplateName))
            {
                yield return new ValidationResult(localizedResourceService.T("NotificationTemplate_Message_ExistingName"), new[] { "NotificationTemplateName" });
            }
        }
    }
}
