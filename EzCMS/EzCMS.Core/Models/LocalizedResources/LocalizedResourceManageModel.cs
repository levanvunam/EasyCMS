using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.Localizes;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.LocalizedResources
{
    public class LocalizedResourceManageModel : IValidatableObject
    {
        #region Constructors

        public LocalizedResourceManageModel()
        {

        }

        public LocalizedResourceManageModel(int languageId)
            : this()
        {
            LanguageId = languageId;
        }

        public LocalizedResourceManageModel(LocalizedResource localizedResource)
            : this(localizedResource.LanguageId)
        {
            Id = localizedResource.Id;

            TextKey = localizedResource.TextKey;
            DefaultValue = localizedResource.DefaultValue;
            TranslatedValue = localizedResource.TranslatedValue;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("LocalizedResource_Field_LanguageId")]
        public int LanguageId { get; set; }

        [Required]
        [LocalizedDisplayName("LocalizedResource_Field_TextKey")]
        public string TextKey { get; set; }

        [LocalizedDisplayName("LocalizedResource_Field_DefaultValue")]
        public string DefaultValue { get; set; }

        [LocalizedDisplayName("LocalizedResource_Field_TranslatedValue")]
        public string TranslatedValue { get; set; }

        #endregion

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var languageService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (languageService.IsTextKeyExisted(Id, LanguageId, TextKey))
            {
                yield return new ValidationResult(localizedResourceService.T("LocalizedResource_Message_ExistingTextKey"), new[] { "TextKey" });
            }
        }

        #endregion
    }
}
