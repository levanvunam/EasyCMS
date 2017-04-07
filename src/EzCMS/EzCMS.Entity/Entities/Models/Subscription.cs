using System;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Entity.Entities.Models
{
    public class Subscription : BaseModel
    {
        public string Email { get; set; }

        public SubscriptionEnums.SubscriptionModule Module { get; set; }

        public string Parameters { get; set; }

        public bool Active { get; set; }

        public DateTime? DeactivatedDate { get; set; }

        public int? ContactId { get; set; }

        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }
    }
}