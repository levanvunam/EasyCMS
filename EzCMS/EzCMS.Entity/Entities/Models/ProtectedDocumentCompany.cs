using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class ProtectedDocumentCompany : BaseModel
    {
        public int ProtectedDocumentId { get; set; }

        [ForeignKey("ProtectedDocumentId")]
        public virtual ProtectedDocument ProtectedDocument { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
    }
}