using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Company : BaseModel
    {
        [StringLength(512)]
        [Required]
        public string Name { get; set; }

        public int? CompanyTypeId { get; set; }

        [ForeignKey("CompanyTypeId")]
        public virtual CompanyType CompanyType { get; set; }

        public virtual ICollection<ProtectedDocumentCompany> ProtectedDocumentCompanies { get; set; }
    }
}