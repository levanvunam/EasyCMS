using System.Collections.Generic;
using System.Linq;
using EzCMS.Core.Models.LocationTypes.Widgets;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Locations.Widgets
{
    public class LocationWidget
    {
        #region Constructors

        public LocationWidget()
        {

        }

        public LocationWidget(Location location)
            : this()
        {
            Id = location.Id;
            Name = location.Name;
            PinImage = location.PinImage;
            AddressLine1 = location.AddressLine1;
            AddressLine2 = location.AddressLine2;
            Suburb = location.Suburb;
            State = location.State;
            Postcode = location.Postcode;
            Country = location.Country;
            Latitude = location.Latitude;
            Longitude = location.Longitude;
            Phone = location.Phone;
            Fax = location.Fax;
            Email = location.Email;
            LocationTypes =
                location.LocationLocationTypes.Select(locationLocationType => new LocationTypeWidget
                {
                    Name = locationLocationType.LocationType.Name,
                    PinImage = locationLocationType.LocationType.PinImage
                }).ToList();
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public string PinImage { get; set; }

        public List<LocationTypeWidget> LocationTypes { get; set; }
        
        #region Address

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string Suburb { get; set; }

        public string State { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        #endregion

        #region Phone

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        #endregion

        #endregion
    }
}
