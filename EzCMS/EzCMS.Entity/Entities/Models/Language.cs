using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Language : BaseModel
    {
        [StringLength(512)]
        public string Name { get; set; }

        [StringLength(512)]
        public string Culture { get; set; }

        [StringLength(10)]
        public string Key { get; set; }

        public bool IsDefault { get; set; }

        public virtual ICollection<LocalizedResource> LocalizedResources { get; set; }

        public virtual ICollection<SlideInHelp> SlideInHelps { get; set; }
    }
}