using System.Collections.Generic;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class NoticeType : BaseModel
    {
        public string Name { get; set; }

        public virtual ICollection<Notice> Notices { get; set; }
    }
}