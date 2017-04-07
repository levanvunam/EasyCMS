using System.Linq;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Services.ContactGroups;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Notifications.Setup
{
    public class ContactGroupSetupModel : NotificationSetupModel
    {
        #region Constructors

        private readonly IContactGroupService _contactGroupService;

        public ContactGroupSetupModel()
        {
            _contactGroupService = HostContainer.GetInstance<IContactGroupService>();

            Step = NotificationEnums.NotificationSetupStep.SetupContactGroup;

            ContactSearchModel = new ContactSearchModel();
        }

        public ContactGroupSetupModel(Notification notification)
            : this()
        {
            Id = notification.Id;

            TotalContacts = string.IsNullOrEmpty(notification.ContactQueries)
                ? 0 : _contactGroupService.GetContacts(notification.ContactQueries).Count();
        }

        #endregion

        #region Public Properties

        public ContactSearchModel ContactSearchModel { get; set; }

        public int TotalContacts { get; set; }

        #endregion
    }
}
