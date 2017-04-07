using System;
using System.Collections.Generic;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Services.Contacts;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Notifications
{
    public class NotificationDetailModel
    {
        public NotificationDetailModel()
        {

        }

        public NotificationDetailModel(Notification notification)
            : this()
        {
            var contactService = HostContainer.GetInstance<IContactService>();

            Id = notification.Id;

            Parameters = notification.Parameters;
            ContactQueries = contactService.GetContactSearchDetailsModels(notification.ContactQueries);
            Module = notification.Module;
            NotificationSubject = notification.NotificationSubject;
            NotificationBody = notification.NotificationBody;
            SendTime = notification.SendTime;

            Created = notification.Created;
            CreatedBy = notification.CreatedBy;
            LastUpdate = notification.LastUpdate;
            LastUpdateBy = notification.LastUpdateBy;
        }

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("Notification_Field_Parameters")]
        public string Parameters { get; set; }

        [LocalizedDisplayName("Notification_Field_ContactQueries")]
        public IEnumerable<ContactSearchDetailsModel> ContactQueries { get; set; }

        [LocalizedDisplayName("Notification_Field_Module")]
        public NotificationEnums.NotificationModule Module { get; set; }

        [LocalizedDisplayName("Notification_Field_NotificationSubject")]
        public string NotificationSubject { get; set; }

        [LocalizedDisplayName("Notification_Field_NotificationBody")]
        public string NotificationBody { get; set; }

        [LocalizedDisplayName("Notification_Field_SendTime")]
        public DateTime? SendTime { get; set; }

        [LocalizedDisplayName("Notification_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("Notification_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("Notification_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("Notification_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        #endregion
    }
}
