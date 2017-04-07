using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.ProtectedDocumentGroups
{
    public class ProtectedDocumentGroupRepository : Repository<ProtectedDocumentGroup>,
        IProtectedDocumentGroupRepository
    {
        public ProtectedDocumentGroupRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Delete mappings between protected document and user groups
        /// </summary>
        /// <param name="protectedDocumentId"></param>
        /// <param name="userGroupIds"></param>
        /// <returns></returns>
        public ResponseModel Delete(int protectedDocumentId, List<int> userGroupIds)
        {
            var entities =
                Fetch(t => t.ProtectedDocumentId == protectedDocumentId && userGroupIds.Contains(t.UserGroupId));

            return Delete(entities);
        }

        /// <summary>
        /// Add new protected document groups
        /// </summary>
        /// <param name="protectedDocumentId"></param>
        /// <param name="userGroupIds"></param>
        /// <returns></returns>
        public ResponseModel Insert(int protectedDocumentId, List<int> userGroupIds)
        {
            return Insert(userGroupIds.Select(userGroupId => new ProtectedDocumentGroup
            {
                ProtectedDocumentId = protectedDocumentId,
                UserGroupId = userGroupId
            }));
        }

        /// <summary>
        /// Delete mapping between protected document and user group
        /// </summary>
        /// <param name="protectedDocumentId"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public ResponseModel Delete(int protectedDocumentId, int userGroupId)
        {
            var entity = FetchFirst(t => t.ProtectedDocumentId == protectedDocumentId && t.UserGroupId == userGroupId);
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