using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.Countries;
using EzCMS.Core.Services.Languages;
using EzCMS.Core.Services.Localizes;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Languages
{
    public class LanguageManageModel : IValidatableObject
    {
        #region Constructors

        public LanguageManageModel()
        {
            var countryService = HostContainer.GetInstance<ICountryService>();

            Countries = countryService.GetCountries();
        }

        public LanguageManageModel(Language language)
            : this()
        {
            Id = language.Id;
            Key = language.Key;
            Name = language.Name;
            Culture = language.Culture;
            RecordOrder = language.RecordOrder;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [StringLength(5)]
        [LocalizedDisplayName("Language_Field_Key")]
        public string Key { get; set; }

        [Required]
        [StringLength(512)]
        [LocalizedDisplayName("Language_Field_Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(512)]
        [LocalizedDisplayName("Language_Field_Culture")]
        public string Culture { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        [LocalizedDisplayName("Language_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var languageService = HostContainer.GetInstance<ILanguageService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (languageService.IsKeyExisted(Id, Key))
            {
                yield return new ValidationResult(localizedResourceService.T("Language_Message_ExistingKey"), new[] { "Key" });
            }
        }

        #endregion
    }
}
