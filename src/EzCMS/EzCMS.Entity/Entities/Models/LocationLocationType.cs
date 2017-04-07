using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class LocationLocationType : BaseModel
    {
        public int LocationId { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }

        public int LocationTypeId { get; set; }

        [ForeignKey("LocationTypeId")]
        public virtual LocationType LocationType { get; set; }
    }
}