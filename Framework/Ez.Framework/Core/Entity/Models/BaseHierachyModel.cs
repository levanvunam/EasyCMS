using System.ComponentModel.DataAnnotations.Schema;

namespace Ez.Framework.Core.Entity.Models
{
    public class BaseHierachyModel<T> : BaseModel where T : class
    {
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual T Parent { get; set; }

        public string Hierarchy { get; set; }
    }
}