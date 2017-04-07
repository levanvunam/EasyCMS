using System;
using Ez.Framework.Models;

namespace EzCMS.Core.Models.Events
{
    public class EventModel : BaseGridModel
    {
        #region Public Properties

        public string Title { get; set; }

        public string EventSummary { get; set; }

        public string EventDescription { get; set; }

        public int? MaxAttendees { get; set; }

        public string RegistrationFullText { get; set; }

        public string RegistrationWaiver { get; set; }

        public DateTime? UpcomingDate { get; set; }

        #endregion
    }
}
