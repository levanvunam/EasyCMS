using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.ProtectedDocumentGroups
{
    [Register(Lifetime.PerInstance)]
    public interface IProtectedDocumentGroupRepository : IRepository<ProtectedDocumentGroup>
    {
        /// <summary>
        /// Delete mappings between protected document and user groups
        /// </summary>
        /// <param name="protectedDocumentId"></param>
        /// <param name="userGroupIds"></param>
        /// <returns></returns>
        ResponseModel Delete(int protectedDocumentId, List<int> userGroupIds);

        /// <summary>
        /// Add new protected document groups
        /// </summary>
        /// <param name="protectedDocumentId"></param>
        /// <param name="userGroupIds"></param>
        /// <returns></returns>
        ResponseModel Insert(int protectedDocumentId, List<int> userGroupIds);

        /// <summary>
        /// Delete mapping between protected document and user group
        /// </summary>
        /// <param name="protectedDocumentId"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        ResponseModel Delete(int protectedDocumentId, int userGroupId);
    }
}