using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.EventCategories;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Events
{
    public class EventManageModel
    {
        #region Constructors

        public EventManageModel()
        {
            var eventCategoryService = HostContainer.GetInstance<IEventCategoryService>();
            EventCategories = eventCategoryService.GetEventCategories();
        }

        public EventManageModel(Event item)
            : this()
        {
            Id = item.Id;

            Title = item.Title;
            EventSummary = item.EventSummary;
            EventDescription = item.EventDescription;
            MaxAttendees = item.MaxAttendees;
            RegistrationFullText = item.RegistrationFullText;
            RegistrationWaiver = item.RegistrationWaiver;

            EventCategoryIds = item.EventEventCategories != null && item.EventEventCategories.Any() ? item.EventEventCategories.Select(x => x.EventCategoryId).ToList() : null;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("Event_Field_Title")]
        public string Title { get; set; }

        [LocalizedDisplayName("Event_Field_Summary")]
        public string EventSummary { get; set; }

        [LocalizedDisplayName("Event_Field_Abstract")]
        public string EventDescription { get; set; }

        [LocalizedDisplayName("Event_Field_MaxAttendees")]
        public int? MaxAttendees { get; set; }

        [LocalizedDisplayName("Event_Field_RegistrationFullText")]
        public string RegistrationFullText { get; set; }

        [LocalizedDisplayName("Event_Field_RegistrationWaiver")]
        public string RegistrationWaiver { get; set; }


        [LocalizedDisplayName("Event_Field_EventCategoryIds")]
        public List<int> EventCategoryIds { get; set; }

        public IEnumerable<SelectListItem> EventCategories { get; set; }

        #endregion
    }
}
