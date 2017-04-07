using System.Collections.Generic;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class RotatingImageGroup : BaseModel
    {
        public string Name { get; set; }

        public string Settings { get; set; }

        public virtual ICollection<RotatingImage> RotatingImages { get; set; }
    }
}