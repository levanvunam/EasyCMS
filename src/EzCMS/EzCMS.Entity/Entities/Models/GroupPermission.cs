using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class GroupPermission : BaseModel
    {
        public int UserGroupId { get; set; }

        [ForeignKey("UserGroupId")]
        public virtual UserGroup UserGroup { get; set; }

        public int PermissionId { get; set; }

        public bool HasPermission { get; set; }
    }
}