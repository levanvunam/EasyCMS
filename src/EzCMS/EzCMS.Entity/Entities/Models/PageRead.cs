using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class PageRead : BaseModel
    {
        public int PageId { get; set; }

        [ForeignKey("PageId")]
        public virtual Page Page { get; set; }

        public int AnonymousContactId { get; set; }

        [ForeignKey("AnonymousContactId")]
        public virtual AnonymousContact AnonymousContact { get; set; }
    }
}