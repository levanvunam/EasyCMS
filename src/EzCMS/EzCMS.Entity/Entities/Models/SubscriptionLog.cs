using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Entity.Entities.Models
{
    public class SubscriptionLog : BaseModel
    {
        public string Parameters { get; set; }

        public SubscriptionEnums.SubscriptionModule Module { get; set; }

        public string ChangeLog { get; set; }

        public bool IsNightlySent { get; set; }

        public bool IsDirectlySent { get; set; }

        public bool Active { get; set; }
    }
}