using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.SlideInHelps
{
    [Register(Lifetime.PerInstance)]
    public interface ISlideInHelpRepository : IRepository<SlideInHelp>
    {
        SlideInHelp Get(string languageKey, string textKey);
    }
}