using System.ComponentModel.DataAnnotations;
using Ez.Framework.Models;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.LocalizedResources
{
    public class LocalizedResourceModel : BaseGridModel
    {
        public LocalizedResourceModel()
        {

        }

        public LocalizedResourceModel(LocalizedResource localizedResource)
            : this()
        {
            LanguageId = localizedResource.LanguageId;
            Language = localizedResource.Language.Key;
            TextKey = localizedResource.TextKey;
            DefaultValue = localizedResource.DefaultValue;
            TranslatedValue = localizedResource.TranslatedValue;

            Created = localizedResource.Created;
            CreatedBy = localizedResource.CreatedBy;
            LastUpdate = localizedResource.LastUpdate;
            LastUpdateBy = localizedResource.LastUpdateBy;
        }

        #region Public Properties

        public string Language { get; set; }

        public int LanguageId { get; set; }

        [Required]
        public string TextKey { get; set; }

        public string DefaultValue { get; set; }

        public string TranslatedValue { get; set; }

        #endregion
    }
}
