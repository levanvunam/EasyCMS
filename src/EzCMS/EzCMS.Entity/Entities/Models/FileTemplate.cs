using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class FileTemplate : BaseModel
    {
        [StringLength(512)]
        public string Name { get; set; }

        [StringLength(512)]
        public string Controller { get; set; }

        [StringLength(512)]
        public string Action { get; set; }

        [StringLength(512)]
        public string Area { get; set; }

        [StringLength(512)]
        public string Parameters { get; set; }

        public int? PageTemplateId { get; set; }

        [ForeignKey("PageTemplateId")]
        public virtual PageTemplate PageTemplate { get; set; }

        public virtual ICollection<Page> Pages { get; set; }
    }
}