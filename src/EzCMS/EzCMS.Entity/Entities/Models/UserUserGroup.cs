using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class UserUserGroup : BaseModel
    {
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int UserGroupId { get; set; }

        [ForeignKey("UserGroupId")]
        public virtual UserGroup UserGroup { get; set; }
    }
}