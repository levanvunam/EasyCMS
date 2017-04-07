using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.LinkTypes;
using EzCMS.Core.Services.Localizes;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.LinkTypes
{
    public class LinkTypeManageModel : IValidatableObject
    {
        #region Constructors

        public LinkTypeManageModel()
        {
        }

        public LinkTypeManageModel(LinkType linkType)
            : this()
        {
            Id = linkType.Id;
            Name = linkType.Name;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("LinkType_Field_Name")]
        public string Name { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var linkTypeService = HostContainer.GetInstance<ILinkTypeService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (linkTypeService.IsTypeExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("LinkType_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}
