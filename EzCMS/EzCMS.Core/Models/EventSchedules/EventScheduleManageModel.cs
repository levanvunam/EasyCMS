using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Services.Events;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.SiteSettings;

namespace EzCMS.Core.Models.EventSchedules
{
    public class EventScheduleManageModel
    {
        #region Constructors

        public EventScheduleManageModel()
        {
            var eventService = HostContainer.GetInstance<IEventService>();
            var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
            var eventConfiguration = siteSettingService.LoadSetting<EventConfiguration>();

            Events = eventService.GetEvents();
            StartHour = eventConfiguration.DefaultStartHour;
            EndHour = eventConfiguration.DefaultEndHour;
        }

        public EventScheduleManageModel(EventSchedule eventSchedule)
            : this()
        {
            Id = eventSchedule.Id;

            EventId = eventSchedule.EventId;
            Location = eventSchedule.Location;
            TimeStart = eventSchedule.TimeStart;
            StartDay = eventSchedule.TimeStart;
            StartHour = eventSchedule.TimeStart;
            TimeEnd = eventSchedule.TimeEnd;
            EndDay = eventSchedule.TimeEnd;
            EndHour = eventSchedule.TimeEnd;
            MaxAttendees = eventSchedule.MaxAttendees;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [RequiredInteger("EventSchedule_Field_EventId")]
        [LocalizedDisplayName("EventSchedule_Field_EventId")]
        public int EventId { get; set; }

        public IEnumerable<SelectListItem> Events { get; set; }

        [LocalizedDisplayName("EventSchedule_Field_Location")]
        public string Location { get; set; }

        [Required]
        [LocalizedDisplayName("EventSchedule_Field_StartDay")]
        public DateTime? StartDay { get; set; }

        [Required]
        [LocalizedDisplayName("EventSchedule_Field_StartHour")]
        public DateTime? StartHour { get; set; }

        [Required]
        [LocalizedDisplayName("EventSchedule_Field_TimeStart")]
        public DateTime? TimeStart { get; set; }

        [RequiredIf("EndHour", "*")]
        [LocalizedDisplayName("EventSchedule_Field_EndDay")]
        public DateTime? EndDay { get; set; }

        [RequiredIf("EndDay", "*")]
        [LocalizedDisplayName("EventSchedule_Field_EndHour")]
        public DateTime? EndHour { get; set; }

        [DateGreaterThan("TimeStart")]
        [LocalizedDisplayName("EventSchedule_Field_TimeEnd")]
        public DateTime? TimeEnd { get; set; }

        [LocalizedDisplayName("EventSchedule_Field_MaxAttendees")]
        public int? MaxAttendees { get; set; }

        #endregion
    }
}
