using System.Collections.Generic;
using System.Linq;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.ProtectedDocuments
{
    public class DocumentPermissionModel
    {
        public DocumentPermissionModel()
        {
            GroupIds = new List<int>();
            CompanyIds = new List<int>();
            CompanyTypeIds = new List<int>();
        }

        public DocumentPermissionModel(ProtectedDocument document)
            : this()
        {
            Path = document.Path;

            GroupIds = document.ProtectedDocumentGroups.Select(d => d.UserGroupId).ToList();
            CompanyIds = document.ProtectedDocumentCompanies.Select(d => d.CompanyId).ToList();
            CompanyTypeIds = document.ProtectedDocumentCompanyTypes.Select(d => d.CompanyTypeId).ToList();
        }

        #region Public Properties

        public string Path { get; set; }

        public List<int> GroupIds { get; set; }

        public List<int> CompanyIds { get; set; }

        public List<int> CompanyTypeIds { get; set; }

        #endregion
    }
}
