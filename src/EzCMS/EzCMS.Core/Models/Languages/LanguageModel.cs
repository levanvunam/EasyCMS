using Ez.Framework.Models;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Languages
{
    public class LanguageModel : BaseGridModel
    {
        public LanguageModel()
        {
            
        }

        public LanguageModel(Language language): this()
        {
            Key = language.Key;
            Name = language.Name;
            Culture = language.Culture;
            IsDefault = language.IsDefault;

            Created = language.Created;
            CreatedBy = language.CreatedBy;
            LastUpdate = language.LastUpdate;
            LastUpdateBy = language.LastUpdateBy;
        }

        #region Public Properties

        public string Key { get; set; }

        public string Name { get; set; }

        public string Culture { get; set; }

        public bool IsDefault { get; set; }

        #endregion
    }
}
