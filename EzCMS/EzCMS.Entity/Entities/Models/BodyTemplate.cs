using Ez.Framework.Core.Entity.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EzCMS.Entity.Entities.Models
{
    public class BodyTemplate : BaseModel
    {
        [StringLength(512)]
        public string Name { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        [StringLength(256)]
        public string ImageUrl { get; set; }

        public string Content { get; set; }

        public virtual ICollection<Page> Pages { get; set; }
    }
}