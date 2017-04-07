using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.EventCategories;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.EventCategories
{
    public class EventCategoryManageModel : IValidatableObject
    {
        #region Constructors

        public EventCategoryManageModel()
        {
        }

        public EventCategoryManageModel(EventCategory eventCategory)
            : this()
        {
            Id = eventCategory.Id;
            Name = eventCategory.Name;
            RecordOrder = eventCategory.RecordOrder;
        }

        #endregion

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("EventCategory_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("EventCategory_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            var categoryService = HostContainer.GetInstance<IEventCategoryService>();

            if (categoryService.IsCategoryExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("EventCategory_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}
