using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.CompanyTypes;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Companies
{
    public class CompanySearchModel
    {
        #region Constructors

        public CompanySearchModel()
        {
            var companyTypeService = HostContainer.GetInstance<ICompanyTypeService>();
            CompanyTypes = companyTypeService.GetCompanyTypes();
        }

        #endregion

        #region Public Properties

        public string Keyword { get; set; }

        public int? CompanyTypeId { get; set; }

        public IEnumerable<SelectListItem> CompanyTypes { get; set; }

        #endregion
    }
}
