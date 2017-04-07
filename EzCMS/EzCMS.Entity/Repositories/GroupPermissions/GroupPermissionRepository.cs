using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.GroupPermissions
{
    public class GroupPermissionRepository : Repository<GroupPermission>, IGroupPermissionRepository
    {
        public GroupPermissionRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        public IQueryable<GroupPermission> GetByGroupId(int id)
        {
            return Fetch(p => p.UserGroupId == id);
        }
    }
}