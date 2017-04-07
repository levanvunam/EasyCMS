using System.Collections.Generic;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class ProtectedDocument : BaseModel
    {
        public string Path { get; set; }

        public virtual ICollection<ProtectedDocumentGroup> ProtectedDocumentGroups { get; set; }

        public virtual ICollection<ProtectedDocumentCompany> ProtectedDocumentCompanies { get; set; }

        public virtual ICollection<ProtectedDocumentCompanyType> ProtectedDocumentCompanyTypes { get; set; }
    }
}