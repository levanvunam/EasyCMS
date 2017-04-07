using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.EventSchedules
{
    public class EventScheduleDetailModel
    {
        public EventScheduleDetailModel()
        {
        }

        public EventScheduleDetailModel(EventSchedule eventSchedule)
            : this()
        {
            Id = eventSchedule.Id;

            EventSchedule = new EventScheduleManageModel(eventSchedule);

            MaxAttendees = eventSchedule.MaxAttendees ?? eventSchedule.Event.MaxAttendees;

            EventTitle = eventSchedule.Event.Title;
            EventSummary = eventSchedule.Event.EventSummary;
            EventDescription = eventSchedule.Event.EventDescription;
            RegistrationFullText = eventSchedule.Event.RegistrationFullText;
            RegistrationWaiver = eventSchedule.Event.RegistrationWaiver;

            Created = eventSchedule.Created;
            CreatedBy = eventSchedule.CreatedBy;
            LastUpdate = eventSchedule.LastUpdate;
            LastUpdateBy = eventSchedule.LastUpdateBy;
        }

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("EventSchedule_Field_MaxAttendees")]
        public int? MaxAttendees { get; set; }

        [LocalizedDisplayName("EventSchedule_Field_EventTitle")]
        public string EventTitle { get; set; }

        [LocalizedDisplayName("EventSchedule_Field_EventSummary")]
        public string EventSummary { get; set; }

        [LocalizedDisplayName("EventSchedule_Field_EventDescription")]
        public string EventDescription { get; set; }

        [LocalizedDisplayName("EventSchedule_Field_RegistrationFullText")]
        public string RegistrationFullText { get; set; }

        [LocalizedDisplayName("EventSchedule_Field_RegistrationWaiver")]
        public string RegistrationWaiver { get; set; }

        [LocalizedDisplayName("EventSchedule_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("EventSchedule_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("EventSchedule_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("EventSchedule_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public EventScheduleManageModel EventSchedule { get; set; }

        #endregion
    }
}
