using Ez.Framework.Core.Entity.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EzCMS.Entity.Entities.Models
{
    public class WidgetTemplate : BaseModel
    {
        [StringLength(512)]
        public string Name { get; set; }

        public string Widgets { get; set; }

        public string Content { get; set; }

        public string Script { get; set; }

        public string Style { get; set; }

        public string FullContent { get; set; }

        [StringLength(512)]
        public string DataType { get; set; }

        [StringLength(512)]
        public string Widget { get; set; }

        public bool IsDefaultTemplate { get; set; }

        public virtual ICollection<WidgetTemplateLog> WidgetTemplateLogs { get; set; }
    }
}