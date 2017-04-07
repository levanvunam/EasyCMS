using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class NewsRead : BaseModel
    {
        public int NewsId { get; set; }

        [ForeignKey("NewsId")]
        public virtual News News { get; set; }

        public int AnonymousContactId { get; set; }

        [ForeignKey("AnonymousContactId")]
        public virtual AnonymousContact AnonymousContact { get; set; }
    }
}