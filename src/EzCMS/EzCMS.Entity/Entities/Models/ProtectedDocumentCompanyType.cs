using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class ProtectedDocumentCompanyType : BaseModel
    {
        public int ProtectedDocumentId { get; set; }

        [ForeignKey("ProtectedDocumentId")]
        public virtual ProtectedDocument ProtectedDocument { get; set; }

        public int CompanyTypeId { get; set; }

        [ForeignKey("CompanyTypeId")]
        public virtual CompanyType CompanyType { get; set; }
    }
}