using Ez.Framework.Core.Entity.RepositoryBase;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.SlideInHelps
{
    public class SlideInHelpRepository : Repository<SlideInHelp>, ISlideInHelpRepository
    {
        public SlideInHelpRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Get localized resource by key and language
        /// </summary>
        /// <param name="languageKey">the language</param>
        /// <param name="textKey">the text</param>
        /// <returns></returns>
        public SlideInHelp Get(string languageKey, string textKey)
        {
            return FetchFirst(l => l.TextKey.Equals(textKey) && l.Language.Key.Equals(languageKey));
        }
    }
}