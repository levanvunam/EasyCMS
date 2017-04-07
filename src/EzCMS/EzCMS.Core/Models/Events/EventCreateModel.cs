using EzCMS.Core.Models.EventSchedules;

namespace EzCMS.Core.Models.Events
{
    public class EventCreateModel
    {
        #region Public Properties

        public EventManageModel Event { get; set; }

        public EventScheduleManageModel EventSchedule { get; set; }

        #endregion
    }
}
