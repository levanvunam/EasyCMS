using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class UserGroup : BaseModel
    {
        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        [StringLength(256)]
        public string RedirectUrl { get; set; }

        public int? ToolbarId { get; set; }

        [ForeignKey("ToolbarId")]
        public virtual Toolbar Toolbar { get; set; }

        public virtual ICollection<GroupPermission> GroupPermissions { get; set; }

        public virtual ICollection<PageSecurity> PageSecurities { get; set; }

        public virtual ICollection<UserUserGroup> UserUserGroups { get; set; }

        public virtual ICollection<ProtectedDocumentGroup> ProtectedDocumentGroups { get; set; }
    }
}