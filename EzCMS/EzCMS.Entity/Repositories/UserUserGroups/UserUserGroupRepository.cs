using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.UserUserGroups
{
    public class UserUserGroupRepository : Repository<UserUserGroup>, IUserUserGroupRepository
    {
        public UserUserGroupRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Delete groups of user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userGroupIds"></param>
        /// <returns></returns>
        public ResponseModel Delete(int userId, IEnumerable<int> userGroupIds)
        {
            var entities = Fetch(m => m.UserId == userId && userGroupIds.Contains(m.UserGroupId));
            return Delete(entities);
        }

        /// <summary>
        /// Delete groups of user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResponseModel Delete(int userId)
        {
            var entities = Fetch(m => m.UserId == userId);

            return Delete(entities);
        }

        /// <summary>
        /// Insert new groups for user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userGroupIds"></param>
        /// <returns></returns>
        public ResponseModel Insert(int userId, IEnumerable<int> userGroupIds)
        {
            return Insert(userGroupIds.Select(id => new UserUserGroup
            {
                UserId = userId,
                UserGroupId = id
            }));
        }

        /// <summary>
        /// Delete mapping between user group and user
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResponseModel Delete(int userGroupId, int userId)
        {
            var entity =
                FetchFirst(userUserGroup => userUserGroup.UserGroupId == userGroupId && userUserGroup.UserId == userId);
            if (entity != null)
            {
                return Delete(entity);
            }

            return new ResponseModel
            {
                Success = true
            };
        }
    }
}