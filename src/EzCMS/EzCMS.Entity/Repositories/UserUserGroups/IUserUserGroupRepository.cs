using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.UserUserGroups
{
    [Register(Lifetime.PerInstance)]
    public interface IUserUserGroupRepository : IRepository<UserUserGroup>
    {
        /// <summary>
        /// Delete mapping by user id and list group id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userGroupIds"></param>
        /// <returns></returns>
        ResponseModel Delete(int userId, IEnumerable<int> userGroupIds);

        /// <summary>
        /// Delete mapping by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ResponseModel Delete(int userId);

        /// <summary>
        /// Delete mapping between user and user group
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        ResponseModel Delete(int userGroupId, int userId);

        /// <summary>
        /// Insert list group id for user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userGroupIds"></param>
        /// <returns></returns>
        ResponseModel Insert(int userId, IEnumerable<int> userGroupIds);
    }
}