using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class PageSecurity : BaseModel
    {
        public int PageId { get; set; }

        [ForeignKey("PageId")]
        public virtual Page Page { get; set; }

        public int GroupId { get; set; }

        [ForeignKey("GroupId")]
        public virtual UserGroup UserGroup { get; set; }

        public bool CanView { get; set; }

        public bool CanEdit { get; set; }
    }
}