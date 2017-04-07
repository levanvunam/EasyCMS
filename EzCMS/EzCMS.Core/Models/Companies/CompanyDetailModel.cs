using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Companies
{
    public class CompanyDetailModel
    {
        public CompanyDetailModel()
        {
        }

        public CompanyDetailModel(Company company)
            : this()
        {
            Id = company.Id;

            Company = new CompanyManageModel(company);
            CompanyType = company.CompanyTypeId.HasValue ? company.CompanyType.Name : string.Empty;

            Created = company.Created;
            CreatedBy = company.CreatedBy;
            LastUpdate = company.LastUpdate;
            LastUpdateBy = company.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("Company_Field_CompanyType")]
        public string CompanyType { get; set; }

        [LocalizedDisplayName("Company_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("Company_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("Company_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("Company_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public CompanyManageModel Company { get; set; }

        #endregion
    }
}
