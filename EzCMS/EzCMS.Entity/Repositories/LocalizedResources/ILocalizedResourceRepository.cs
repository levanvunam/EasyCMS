using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.LocalizedResources
{
    [Register(Lifetime.PerInstance)]
    public interface ILocalizedResourceRepository : IRepository<LocalizedResource>
    {
        /// <summary>
        /// Get resource by language key and text key
        /// </summary>
        /// <param name="languageKey"></param>
        /// <param name="textKey"></param>
        /// <returns></returns>
        LocalizedResource Get(string languageKey, string textKey);
    }
}