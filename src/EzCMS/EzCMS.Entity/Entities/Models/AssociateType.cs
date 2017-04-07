using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class AssociateType : BaseModel
    {
        [StringLength(255)]
        public string Name { get; set; }

        public virtual ICollection<AssociateAssociateType> AssociateAssociateTypes { get; set; }
    }
}