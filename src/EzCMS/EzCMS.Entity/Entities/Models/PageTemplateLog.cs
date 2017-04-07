using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class PageTemplateLog : BaseModel
    {
        [StringLength(512)]
        public string SessionId { get; set; }

        [StringLength(512)]
        public string Name { get; set; }

        public int PageTemplateId { get; set; }

        [ForeignKey("PageTemplateId")]
        public virtual PageTemplate PageTemplate { get; set; }

        public int? ParentId { get; set; }

        public string Content { get; set; }

        public string ChangeLog { get; set; }
    }
}