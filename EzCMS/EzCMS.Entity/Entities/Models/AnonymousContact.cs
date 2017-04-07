using Ez.Framework.Core.Entity.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzCMS.Entity.Entities.Models
{
    public class AnonymousContact : BaseModel
    {
        public int? ContactId { get; set; }

        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }

        public string CookieKey { get; set; }

        public string IpAddress { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public virtual ICollection<PageRead> PageReads { get; set; }

        public virtual ICollection<NewsRead> NewsReads { get; set; }
    }
}