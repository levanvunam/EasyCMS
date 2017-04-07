using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class WidgetTemplateLog : BaseModel
    {
        [StringLength(512)]
        public string SessionId { get; set; }

        public int TemplateId { get; set; }

        [ForeignKey("TemplateId")]
        public virtual WidgetTemplate WidgetTemplate { get; set; }

        [StringLength(512)]
        public string Name { get; set; }

        public string Widgets { get; set; }

        public string Content { get; set; }

        public string Script { get; set; }

        public string Style { get; set; }

        public string FullContent { get; set; }

        [StringLength(512)]
        public string DataType { get; set; }

        public string ChangeLog { get; set; }
    }
}