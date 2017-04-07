using System.Collections.Generic;

namespace EzCMS.Core.Models.EventSchedules.Widgets
{
    public class EventSchedulesWidget
    {
        public EventSchedulesWidget()
        {
            EventSchedules = new List<EventScheduleWidget>();
        }

        #region Public Properties

        public List<EventScheduleWidget> EventSchedules { get; set; }

        #endregion
    }
}