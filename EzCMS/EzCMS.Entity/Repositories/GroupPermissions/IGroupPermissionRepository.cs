using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;
using System.Linq;

namespace EzCMS.Entity.Repositories.GroupPermissions
{
    [Register(Lifetime.PerInstance)]
    public interface IGroupPermissionRepository : IRepository<GroupPermission>
    {
        IQueryable<GroupPermission> GetByGroupId(int id);
    }
}