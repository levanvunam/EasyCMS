using Ez.Framework.Core.Entity.RepositoryBase;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.LocalizedResources
{
    public class LocalizedResourceRepository : Repository<LocalizedResource>, ILocalizedResourceRepository
    {
        public LocalizedResourceRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Get localized resource by key and language
        /// </summary>
        /// <param name="languageKey">the language</param>
        /// <param name="textKey">the text</param>
        /// <returns></returns>
        public LocalizedResource Get(string languageKey, string textKey)
        {
            return FetchFirst(l => l.TextKey.Equals(textKey) && l.Language.Key.Equals(languageKey));
        }
    }
}