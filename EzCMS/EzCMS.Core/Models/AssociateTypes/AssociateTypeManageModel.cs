using Ez.Framework.Core.Attributes;
using Ez.Framework.IoC;
using EzCMS.Core.Services.AssociateTypes;
using EzCMS.Core.Services.Localizes;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.AssociateTypes
{
    public class AssociateTypeManageModel : IValidatableObject
    {
        #region Constructors

        public AssociateTypeManageModel()
        {
        }

        public AssociateTypeManageModel(AssociateType associateType)
            : this()
        {
            Id = associateType.Id;
            Name = associateType.Name;
            RecordOrder = associateType.RecordOrder;
        }

        #endregion

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("AssociateType_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("AssociateType_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            var associateTypeService = HostContainer.GetInstance<IAssociateTypeService>();

            if (associateTypeService.IsAssociateTypeExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("AssociateType_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}
