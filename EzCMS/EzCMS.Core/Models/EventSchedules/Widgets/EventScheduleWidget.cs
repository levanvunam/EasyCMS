using System;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.EventSchedules;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.EventSchedules.Widgets
{
    public class EventScheduleWidget
    {
        public EventScheduleWidget()
        {
        }

        public EventScheduleWidget(EventSchedule eventSchedule)
            : this()
        {
            Id = eventSchedule.Id;

            EventId = eventSchedule.EventId;
            Location = eventSchedule.Location;
            TimeStart = eventSchedule.TimeStart;
            TimeEnd = eventSchedule.TimeEnd;
            MaxAttendees = eventSchedule.MaxAttendees ?? eventSchedule.Event.MaxAttendees;

            EventTitle = eventSchedule.Event.Title;
            EventSummary = eventSchedule.Event.EventSummary;
            EventDescription = eventSchedule.Event.EventDescription;
            RegistrationFullText = eventSchedule.Event.RegistrationFullText;
            RegistrationWaiver = eventSchedule.Event.RegistrationWaiver;
        }

        public EventScheduleWidget(EventSchedule eventSchedule, string dateFormat, string timeFormat)
            : this(eventSchedule)
        {
            var eventScheduleService = HostContainer.GetInstance<IEventScheduleService>();

            TimeFrame = eventScheduleService.GetEventScheduleTimeFrame(TimeStart, TimeEnd, dateFormat, timeFormat);
        }

        #region Public Properties

        public int Id { get; set; }

        public int EventId { get; set; }

        public string Location { get; set; }

        public DateTime TimeStart { get; set; }

        public DateTime? TimeEnd { get; set; }

        public string TimeFrame { get; set; }

        public int? MaxAttendees { get; set; }

        public string EventTitle { get; set; }

        public string EventSummary { get; set; }

        public string EventDescription { get; set; }

        public string RegistrationFullText { get; set; }

        public string RegistrationWaiver { get; set; }

        #endregion
    }
}