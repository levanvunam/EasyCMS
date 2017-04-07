using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.EventCategories
{
    public class EventCategoryDetailModel
    {
        public EventCategoryDetailModel()
        {
        }

        public EventCategoryDetailModel(EventCategory eventCategory)
            : this()
        {
            Id = eventCategory.Id;

            EventCategory = new EventCategoryManageModel(eventCategory);

            Created = eventCategory.Created;
            CreatedBy = eventCategory.CreatedBy;
            LastUpdate = eventCategory.LastUpdate;
            LastUpdateBy = eventCategory.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("EventCategory_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("EventCategory_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("EventCategory_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("EventCategory_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public EventCategoryManageModel EventCategory { get; set; }

        #endregion
    }
}
