using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Models.LocationTypes.Widgets;
using EzCMS.Core.Services.LocationTypes;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Locations.Widgets
{
    public class LocatorWidget
    {
        #region Constructors

        public LocatorWidget()
        {
            
        }

        public LocatorWidget(string country)
            : this()
        {
            var locationTypeService = HostContainer.GetInstance<ILocationTypeService>();
            LocationTypes = locationTypeService.GetAll().Select(locationType => new LocationTypeWidget
            {
                Id = locationType.Id,
                Name = locationType.Name,
                PinImage = locationType.PinImage
            }).ToList();
            Country = country;

        }

        #endregion

        #region Public Properties

        public List<LocationTypeWidget> LocationTypes { get; set; }

        public string Country { get; set; }

        #endregion
    }
}
