using System.Collections.Generic;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Event : BaseModel
    {
        public string Title { get; set; }

        public string EventSummary { get; set; }

        public string EventDescription { get; set; }

        public int? MaxAttendees { get; set; }

        public string RegistrationFullText { get; set; }

        public string RegistrationWaiver { get; set; }

        public virtual ICollection<EventSchedule> EventSchedules { get; set; }

        public virtual ICollection<EventEventCategory> EventEventCategories { get; set; }
    }
}