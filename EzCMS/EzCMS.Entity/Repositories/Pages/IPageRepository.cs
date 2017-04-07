using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.Pages
{
    [Register(Lifetime.PerInstance)]
    public interface IPageRepository : IHierarchyRepository<Page>
    {
    }
}