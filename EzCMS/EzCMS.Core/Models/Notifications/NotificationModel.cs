using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.Utilities;
using System;
using Ez.Framework.Models;

namespace EzCMS.Core.Models.Notifications
{
    public class NotificationModel : BaseGridModel
    {
        #region Constructors

        public NotificationModel()
        {
        }

        public NotificationModel(Notification notification)
            : base(notification)
        {
            Id = notification.Id;
            Parameters = notification.Parameters;
            ContactQueries = notification.ContactQueries;
            NotifiedContacts = notification.NotifiedContacts;
            Module = notification.Module;
            NotificationSubject = notification.NotificationSubject;
            NotificationBody = notification.NotificationBody;
            SendTime = notification.SendTime;
        }

        #endregion

        #region Public Properties

        public string Parameters { get; set; }

        public string ContactQueries { get; set; }

        public string NotifiedContacts { get; set; }

        public NotificationEnums.NotificationModule Module { get; set; }

        public string ModuleName
        {
            get { return Module.GetEnumName(); }
        }

        public string NotificationSubject { get; set; }

        public string NotificationBody { get; set; }

        public DateTime? SendTime { get; set; }

        public bool Active { get; set; }

        #endregion
    }
}
