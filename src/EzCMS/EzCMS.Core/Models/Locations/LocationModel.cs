using System.Collections.Generic;
using Ez.Framework.Models;

namespace EzCMS.Core.Models.Locations
{
    public class LocationModel : BaseGridModel
    {
        #region Public Properties

        public string Name { get; set; }

        public string ContactName { get; set; }

        public string ContactTitle { get; set; }

        public IEnumerable<string> Types { get; set; }

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

        public string TrainingAffiliation { get; set; }

        public string PinImage { get; set; }

        public string OpeningHoursWeekdays { get; set; }

        public string OpeningHoursSaturday { get; set; }

        public string OpeningHoursSunday { get; set; }

        public string TimeZone { get; set; }

        #endregion
    }
}
