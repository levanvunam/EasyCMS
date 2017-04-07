using System;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class EventSchedule : BaseModel
    {
        public string Location { get; set; }

        public DateTime TimeStart { get; set; }

        public DateTime? TimeEnd { get; set; }

        public int? MaxAttendees { get; set; }

        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }
    }
}