using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.Mvc.Helpers;
using EzCMS.Core.Services.Companies;
using EzCMS.Core.Services.CompanyTypes;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.ProtectedDocuments;
using EzCMS.Core.Services.UserGroups;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.ProtectedDocuments
{
    public class DocumentPermissionManageModel : IValidatableObject
    {
        private readonly ICompanyService _companyService;
        private readonly ICompanyTypeService _companyTypeService;
        private readonly IUserGroupService _userGroupService;
        private readonly IDocumentService _documentService;
        public DocumentPermissionManageModel()
        {
            _companyService = HostContainer.GetInstance<ICompanyService>();
            _companyTypeService = HostContainer.GetInstance<ICompanyTypeService>();
            _userGroupService = HostContainer.GetInstance<IUserGroupService>();
            _documentService = HostContainer.GetInstance<IDocumentService>();

            Groups = _userGroupService.GetUserGroups();
            Companies = _companyService.GetCompanies();
            CompanyTypes = _companyTypeService.GetCompanyTypes();
        }

        public DocumentPermissionManageModel(string encryptedPath)
            : this()
        {
            EncryptedPath = encryptedPath;
            RelativePath = encryptedPath.GetUniqueLinkInput();

            var document = _documentService.GetByEncryptPath(encryptedPath);

            if (document != null)
            {
                GroupIds = document.ProtectedDocumentGroups.Select(d => d.UserGroupId).ToList();
                Groups = _userGroupService.GetUserGroups(GroupIds);

                CompanyIds = document.ProtectedDocumentCompanies.Select(d => d.CompanyId).ToList();
                Companies = _companyService.GetCompanies();

                CompanyTypeIds = document.ProtectedDocumentCompanyTypes.Select(d => d.CompanyTypeId).ToList();
                CompanyTypes = _companyTypeService.GetCompanyTypes();
            }

        }

        #region Public Properties

        [LocalizedDisplayName("ProtectedDocument_Field_EncryptedPath")]
        public string EncryptedPath { get; set; }

        [LocalizedDisplayName("ProtectedDocument_Field_RelativePath")]
        public string RelativePath { get; set; }

        [LocalizedDisplayName("ProtectedDocument_Field_GroupIds")]
        public List<int> GroupIds { get; set; }

        public IEnumerable<SelectListItem> Groups { get; set; }

        [LocalizedDisplayName("ProtectedDocument_Field_CompanyIds")]
        public List<int> CompanyIds { get; set; }

        public IEnumerable<SelectListItem> Companies { get; set; }

        [LocalizedDisplayName("ProtectedDocument_Field_CompanyTypeIds")]
        public List<int> CompanyTypeIds { get; set; }

        public IEnumerable<SelectListItem> CompanyTypes { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (!EncryptedPath.Equals(RelativePath.GetUniqueLink()))
            {
                yield return new ValidationResult(localizedResourceService.T("ProtectedDocument_Message_InvalidPathMapping"), new[] { "Email" });
            }
        }
    }
}
