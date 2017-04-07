using System;
using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Entity.Entities.Models
{
    public class Notification : BaseModel
    {
        public string Parameters { get; set; }

        public string ContactQueries { get; set; }

        public string NotifiedContacts { get; set; }

        public NotificationEnums.NotificationModule Module { get; set; }

        public string NotificationSubject { get; set; }

        public string NotificationBody { get; set; }

        public DateTime? SendTime { get; set; }

        public bool Active { get; set; }
    }
}