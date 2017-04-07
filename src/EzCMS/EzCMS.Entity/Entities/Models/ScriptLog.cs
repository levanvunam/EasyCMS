using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class ScriptLog : BaseModel
    {
        [StringLength(512)]
        public string SessionId { get; set; }

        public int ScriptId { get; set; }

        [ForeignKey("ScriptId")]
        public virtual Script Script { get; set; }

        [StringLength(512)]
        public string Name { get; set; }

        public string Content { get; set; }

        [StringLength(512)]
        public string DataType { get; set; }

        public string ChangeLog { get; set; }
    }
}