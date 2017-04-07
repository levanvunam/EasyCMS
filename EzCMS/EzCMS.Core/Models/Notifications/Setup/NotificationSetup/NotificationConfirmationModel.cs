using System;
using System.Linq;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.ContactGroups;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Notifications.Setup.NotificationSetup
{
    public class NotificationConfirmationModel
    {
        #region Constructors

        private readonly IContactGroupService _contactGroupService;

        public NotificationConfirmationModel()
        {
            _contactGroupService = HostContainer.GetInstance<IContactGroupService>();
        }

        public NotificationConfirmationModel(Notification notification)
            : this()
        {
            Id = notification.Id;

            var contacts = _contactGroupService.GetContacts(notification.ContactQueries);
            TotalRecipients = contacts.Count();

            NotificationSubject = notification.NotificationSubject;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("Notification_Field_NotificationSubject")]
        public string NotificationSubject { get; set; }

        [LocalizedDisplayName("Notification_Field_NotificationTemplateName")]
        public string NotificationTemplateName { get; set; }

        [LocalizedDisplayName("Notification_Field_ContactGroupName")]
        public string ContactGroupName { get; set; }

        [LocalizedDisplayName("Notification_Field_TotalRecipients")]
        public int TotalRecipients { get; set; }

        public DateTime? SendTime { get; set; }

        #endregion
    }
}
