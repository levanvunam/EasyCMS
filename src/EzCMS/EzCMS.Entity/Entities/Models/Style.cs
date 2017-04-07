using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Style : BaseModel
    {
        [StringLength(512)]
        public string Name { get; set; }

        public string CdnUrl { get; set; }

        public string Content { get; set; }

        public bool IncludeIntoEditor { get; set; }

        public virtual ICollection<StyleLog> StyleLogs { get; set; }

        public virtual ICollection<Form> Forms { get; set; }
    }
}