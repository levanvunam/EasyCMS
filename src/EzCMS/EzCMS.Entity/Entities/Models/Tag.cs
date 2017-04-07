using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Tag : BaseModel
    {
        [StringLength(512)]
        public string Name { get; set; }

        public virtual ICollection<PageTag> PageTags { get; set; }
    }
}