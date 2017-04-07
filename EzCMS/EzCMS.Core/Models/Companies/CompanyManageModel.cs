using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.Companies;
using EzCMS.Core.Services.CompanyTypes;
using EzCMS.Core.Services.Localizes;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Companies
{
    public class CompanyManageModel : IValidatableObject
    {
        #region Constructors

        public CompanyManageModel()
        {
            var companyTypeService = HostContainer.GetInstance<ICompanyTypeService>();

            CompanyTypes = companyTypeService.GetCompanyTypes();
        }

        public CompanyManageModel(Company company)
            : this()
        {
            Id = company.Id;
            Name = company.Name;
            CompanyTypeId = company.CompanyTypeId;
            RecordOrder = company.RecordOrder;
        }

        #endregion

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("Company_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("Company_Field_CompanyTypeId")]
        public int? CompanyTypeId { get; set; }

        public IEnumerable<SelectListItem> CompanyTypes { get; set; }

        [LocalizedDisplayName("Company_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var companyService = HostContainer.GetInstance<ICompanyService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (companyService.IsNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("Company_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}
