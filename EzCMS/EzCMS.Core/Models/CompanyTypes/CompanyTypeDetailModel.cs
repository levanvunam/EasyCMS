using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.CompanyTypes
{
    public class CompanyTypeDetailModel
    {
        public CompanyTypeDetailModel()
        {
        }

        public CompanyTypeDetailModel(CompanyType companyType)
            : this()
        {
            Id = companyType.Id;

            CompanyType = new CompanyTypeManageModel(companyType);

            Created = companyType.Created;
            CreatedBy = companyType.CreatedBy;
            LastUpdate = companyType.LastUpdate;
            LastUpdateBy = companyType.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("CompanyType_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("CompanyType_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("CompanyType_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("CompanyType_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public CompanyTypeManageModel CompanyType { get; set; }

        #endregion
    }
}
