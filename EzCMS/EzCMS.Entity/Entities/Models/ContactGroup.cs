using Ez.Framework.Core.Entity.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EzCMS.Entity.Entities.Models
{
    public class ContactGroup : BaseModel
    {
        [StringLength(512)]
        public string Name { get; set; }

        public string Queries { get; set; }

        public bool Active { get; set; }

        public virtual ICollection<ContactGroupContact> ContactGroupContacts { get; set; }
    }
}