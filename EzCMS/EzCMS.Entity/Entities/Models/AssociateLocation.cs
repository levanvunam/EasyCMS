using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class AssociateLocation : BaseModel
    {
        public int AssociateId { get; set; }

        [ForeignKey("AssociateId")]
        public virtual Associate Associate { get; set; }

        public int LocationId { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
    }
}