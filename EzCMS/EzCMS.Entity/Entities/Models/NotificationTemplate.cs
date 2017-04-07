using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Entity.Entities.Models
{
    public class NotificationTemplate : BaseModel
    {
        public string Name { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsDefaultTemplate { get; set; }

        public NotificationEnums.NotificationModule Module { get; set; }
    }
}