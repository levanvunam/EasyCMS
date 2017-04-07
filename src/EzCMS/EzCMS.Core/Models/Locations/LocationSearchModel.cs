using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.LocationTypes;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Locations
{
    public class LocationSearchModel
    {
        public LocationSearchModel()
        {
            var locationTypeService = HostContainer.GetInstance<ILocationTypeService>();
            LocationTypes = locationTypeService.GetLocationTypes();
        }

        #region Public Properties

        public string Keyword { get; set; }

        public int? AssociateId { get; set; }

        public List<int> LocationTypeIds { get; set; }

        public IEnumerable<SelectListItem> LocationTypes { get; set; }

        #endregion
    }
}
