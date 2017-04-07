using Ez.Framework.Core.Entity.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzCMS.Entity.Entities.Models
{
    public class ContactGroupContact : BaseModel
    {
        public int ContactId { get; set; }

        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }

        public int ContactGroupId { get; set; }

        [ForeignKey("ContactGroupId")]
        public virtual ContactGroup ContactGroup { get; set; }
    }
}