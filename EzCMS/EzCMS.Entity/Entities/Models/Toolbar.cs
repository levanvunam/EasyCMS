using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Toolbar : BaseModel
    {
        [StringLength(512)]
        public string Name { get; set; }

        public string BasicToolbar { get; set; }

        public string PageToolbar { get; set; }

        public bool IsDefault { get; set; }

        public virtual ICollection<UserGroup> UserGroups { get; set; }
    }
}