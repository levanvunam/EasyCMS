using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.AssociateTypes;
using EzCMS.Core.Services.CompanyTypes;
using EzCMS.Core.Services.Locations;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Associates
{
    public class AssociateSearchModel
    {
        public AssociateSearchModel()
        {
            var associateTypeService = HostContainer.GetInstance<IAssociateTypeService>();
            var companyTypeService = HostContainer.GetInstance<ICompanyTypeService>();
            var locationService = HostContainer.GetInstance<ILocationService>();

            AssociateTypeIds = new List<int>();
            AssociateTypes = associateTypeService.GetAssociateTypes();

            LocationIds = new List<int>();
            Locations = locationService.GetLocations();

            CompanyTypeIds = new List<int>();
            CompanyTypes = companyTypeService.GetCompanyTypes();
        }

        #region Public Properties

        public string Keyword { get; set; }

        public List<int> AssociateTypeIds { get; set; }

        public IEnumerable<SelectListItem> AssociateTypes { get; set; }

        public List<int> LocationIds { get; set; }

        public IEnumerable<SelectListItem> Locations { get; set; }

        public List<int> CompanyTypeIds { get; set; }

        public IEnumerable<SelectListItem> CompanyTypes { get; set; }

        #endregion
    }
}
