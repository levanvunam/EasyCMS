using EzCMS.Entity.Entities.Models;
using System;
using Ez.Framework.Models;

namespace EzCMS.Core.Models.EventSchedules
{
    public class EventScheduleModel : BaseGridModel
    {
        #region Constructors

        public EventScheduleModel()
        {
        }

        public EventScheduleModel(EventSchedule eventSchedule)
            : this()
        {
            Id = eventSchedule.Id;

            EventId = eventSchedule.EventId;
            EventTitle = eventSchedule.Event.Title;
            Location = eventSchedule.Location;
            TimeStart = eventSchedule.TimeStart;
            TimeEnd = eventSchedule.TimeEnd;
            MaxAttendees = eventSchedule.MaxAttendees;

            RecordOrder = eventSchedule.RecordOrder;
            Created = eventSchedule.Created;
            CreatedBy = eventSchedule.CreatedBy;
            LastUpdate = eventSchedule.LastUpdate;
            LastUpdateBy = eventSchedule.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public int EventId { get; set; }

        public string EventTitle { get; set; }

        public string Location { get; set; }

        public DateTime TimeStart { get; set; }

        public DateTime? TimeEnd { get; set; }

        public int? MaxAttendees { get; set; }

        #endregion
    }
}
