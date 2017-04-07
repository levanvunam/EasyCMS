using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.PageSecurities
{
    [Register(Lifetime.PerInstance)]
    public interface IPageSecurityRepository : IRepository<PageSecurity>
    {
        /// <summary>
        /// Delete mapping between user group and page security
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        ResponseModel Delete(int userGroupId, int pageId);
    }
}