using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class StyleLog : BaseModel
    {
        [StringLength(512)]
        public string SessionId { get; set; }

        public int StyleId { get; set; }

        [ForeignKey("StyleId")]
        public virtual Style Style { get; set; }

        [StringLength(512)]
        public string Name { get; set; }

        public string Content { get; set; }

        public string CdnUrl { get; set; }

        public bool IncludeIntoEditor { get; set; }

        public string ChangeLog { get; set; }
    }
}