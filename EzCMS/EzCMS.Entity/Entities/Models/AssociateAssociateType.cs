using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class AssociateAssociateType : BaseModel
    {
        public int AssociateId { get; set; }

        [ForeignKey("AssociateId")]
        public virtual Associate Associate { get; set; }

        public int AssociateTypeId { get; set; }

        [ForeignKey("AssociateTypeId")]
        public virtual AssociateType AssociateType { get; set; }
    }
}