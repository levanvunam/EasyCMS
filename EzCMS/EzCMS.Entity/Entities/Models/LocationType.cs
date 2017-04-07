using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class LocationType : BaseModel
    {
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string PinImage { get; set; }

        public virtual ICollection<LocationLocationType> LocationLocationTypes { get; set; }
    }
}