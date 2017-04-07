using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Entity.Entities.Models
{
    public class SubscriptionTemplate : BaseModel
    {
        public string Name { get; set; }

        public string Body { get; set; }

        public SubscriptionEnums.SubscriptionModule Module { get; set; }
    }
}