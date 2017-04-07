using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Script : BaseModel
    {
        [StringLength(512)]
        public string Name { get; set; }

        public string Content { get; set; }

        public virtual ICollection<ScriptLog> ScriptLogs { get; set; }
    }
}