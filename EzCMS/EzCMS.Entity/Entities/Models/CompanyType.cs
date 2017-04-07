using Ez.Framework.Core.Entity.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EzCMS.Entity.Entities.Models
{
    public class CompanyType : BaseModel
    {
        [StringLength(512)]
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Company> Companies { get; set; }

        public virtual ICollection<ProtectedDocumentCompanyType> ProtectedDocumentCompanyTypes { get; set; }

        public virtual ICollection<AssociateCompanyType> AssociateCompanyTypes { get; set; }
    }
}