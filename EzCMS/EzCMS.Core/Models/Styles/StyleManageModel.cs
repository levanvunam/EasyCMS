using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.Styles;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Styles
{
    public class StyleManageModel : IValidatableObject
    {
        public StyleManageModel()
        {

        }

        public StyleManageModel(Style style)
        {
            Id = style.Id;
            Name = style.Name;
            CdnUrl = style.CdnUrl;
            Content = style.Content;
            IncludeIntoEditor = style.IncludeIntoEditor;
        }

        public StyleManageModel(StyleLog log)
            : this()
        {
            Id = log.StyleId;
            Name = log.Name;
            CdnUrl = log.CdnUrl;
            Content = log.Content;
            IncludeIntoEditor = log.IncludeIntoEditor;
        }

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("Style_Field_Name")]
        public string Name { get; set; }

        [RequiredIf("Content", null)]
        [LocalizedDisplayName("Style_Field_CdnUrl")]
        public string CdnUrl { get; set; }

        [RequiredIf("CdnUrl", null)]
        [LocalizedDisplayName("Style_Field_Content")]
        public string Content { get; set; }

        [LocalizedDisplayName("Style_Field_IncludeIntoEditor")]
        public bool IncludeIntoEditor { get; set; }
        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var styleService = HostContainer.GetInstance<IStyleService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (styleService.IsStyleNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("Style_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}
