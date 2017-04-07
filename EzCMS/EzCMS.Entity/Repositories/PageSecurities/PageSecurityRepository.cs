using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.PageSecurities
{
    public class PageSecurityRepository : Repository<PageSecurity>, IPageSecurityRepository
    {
        public PageSecurityRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Delete mapping between user group and page security
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ResponseModel Delete(int userGroupId, int pageId)
        {
            var entity = FetchFirst(pageSecurity => pageSecurity.GroupId == userGroupId && pageSecurity.PageId == pageId);
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