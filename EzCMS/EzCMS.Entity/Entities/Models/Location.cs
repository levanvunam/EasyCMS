using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Location : BaseModel
    {
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string TrainingAffiliation { get; set; }

        [StringLength(255)]
        public string PinImage { get; set; }

        [StringLength(100)]
        public string ContactName { get; set; }

        [StringLength(100)]
        public string ContactTitle { get; set; }

        [StringLength(100)]
        public string OpeningHoursWeekdays { get; set; }

        [StringLength(100)]
        public string OpeningHoursSaturday { get; set; }

        [StringLength(100)]
        public string OpeningHoursSunday { get; set; }

        public string TimeZone { get; set; }

        public virtual ICollection<AssociateLocation> AssociateLocations { get; set; }

        public virtual ICollection<LocationLocationType> LocationLocationTypes { get; set; }

        #region Address

        [StringLength(255)]
        public string AddressLine1 { get; set; }

        [StringLength(255)]
        public string AddressLine2 { get; set; }

        [StringLength(100)]
        public string Suburb { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(50)]
        public string Postcode { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        #endregion

        #region Phone

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Fax { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        #endregion
    }
}