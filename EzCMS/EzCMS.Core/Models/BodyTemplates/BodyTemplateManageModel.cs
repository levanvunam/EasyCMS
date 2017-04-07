using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.BodyTemplates;
using EzCMS.Core.Services.Localizes;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.BodyTemplates
{
    public class BodyTemplateManageModel : IValidatableObject
    {
        #region Constructors

        private readonly IBodyTemplateService _bodyTemplateService;
        public BodyTemplateManageModel()
        {
            _bodyTemplateService = HostContainer.GetInstance<IBodyTemplateService>();
        }

        public BodyTemplateManageModel(BodyTemplate bodyTemplate)
            : this()
        {
            Id = bodyTemplate.Id;
            Name = bodyTemplate.Name;
            ImageUrl = bodyTemplate.ImageUrl;
            Description = bodyTemplate.Description;
            Content = bodyTemplate.Content;
        }

        #endregion

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [StringLength(512)]
        [LocalizedDisplayName("BodyTemplate_Field_Name")]
        public string Name { get; set; }

        [StringLength(512)]
        [LocalizedDisplayName("BodyTemplate_Field_Description")]
        public string Description { get; set; }

        [StringLength(256)]
        [LocalizedDisplayName("BodyTemplate_Field_ImageUrl")]
        public string ImageUrl { get; set; }

        [Required]
        [LocalizedDisplayName("BodyTemplate_Field_Content")]
        public string Content { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (_bodyTemplateService.IsNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("BodyTemplate_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}
