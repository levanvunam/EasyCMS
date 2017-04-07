using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.LocationTypes;
using EzCMS.Core.Services.Localizes;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.LocationTypes
{
    public class LocationTypeManageModel : IValidatableObject
    {
        #region Constructors

        public LocationTypeManageModel()
        {
        }

        public LocationTypeManageModel(LocationType associateType)
            : this()
        {
            Id = associateType.Id;
            Name = associateType.Name;
            PinImage = associateType.PinImage;
            RecordOrder = associateType.RecordOrder;
        }

        #endregion

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("LocationType_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("LocationType_Field_PinImage")]
        public string PinImage { get; set; }

        [LocalizedDisplayName("LocationType_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            var locationTypeService = HostContainer.GetInstance<ILocationTypeService>();

            if (locationTypeService.IsLocationTypeExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("LocationType_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}
