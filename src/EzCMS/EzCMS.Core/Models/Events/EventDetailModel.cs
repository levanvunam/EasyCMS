using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Events
{
    public class EventDetailModel
    {
        public EventDetailModel()
        {

        }

        public EventDetailModel(Event item)
            : this()
        {
            Id = item.Id;

            Event = new EventManageModel(item);

            Created = item.Created;
            CreatedBy = item.CreatedBy;
            LastUpdate = item.LastUpdate;
            LastUpdateBy = item.LastUpdateBy;
        }

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("Event_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("Event_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("Event_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("Event_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public EventManageModel Event { get; set; }

        #endregion
    }
}
