using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.ClientNavigations
{
    [Register(Lifetime.PerInstance)]
    public interface IClientNavigationRepository : IHierarchyRepository<ClientNavigation>
    {
        /// <summary>
        /// Save the client menu from page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        ResponseModel SaveFromPage(Page page);

        /// <summary>
        /// Delete all client menus of page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        ResponseModel DeleteNavigationsOfPage(Page page);
    }
}