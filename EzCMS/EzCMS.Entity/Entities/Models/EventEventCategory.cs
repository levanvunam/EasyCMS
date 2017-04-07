using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class EventEventCategory : BaseModel
    {
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        public int EventCategoryId { get; set; }

        [ForeignKey("EventCategoryId")]
        public virtual EventCategory EventCategory { get; set; }
    }
}