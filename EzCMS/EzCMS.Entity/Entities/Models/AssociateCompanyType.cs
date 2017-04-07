using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class AssociateCompanyType : BaseModel
    {
        public int AssociateId { get; set; }

        [ForeignKey("AssociateId")]
        public virtual Associate Associate { get; set; }

        public int CompanyTypeId { get; set; }

        [ForeignKey("CompanyTypeId")]
        public virtual CompanyType CompanyType { get; set; }
    }
}