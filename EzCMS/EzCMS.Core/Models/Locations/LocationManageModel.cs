using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Core.Enums;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Time;
using Ez.Framework.IoC;
using EzCMS.Core.Services.Countries;
using EzCMS.Core.Services.LocationTypes;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Locations
{
    public class LocationManageModel
    {
        #region Constructors

        private readonly ILocationTypeService _locationTypeService;
        public LocationManageModel()
        {
            _locationTypeService = HostContainer.GetInstance<ILocationTypeService>();
            var countryService = HostContainer.GetInstance<ICountryService>();

            LocationTypes = _locationTypeService.GetLocationTypes();
            Countries = countryService.GetCountries();
            TimeZones = TimeZoneUtilities.GetTimeZones();
            States = EnumUtilities.GenerateSelectListItems<CommonEnums.AustraliaState>(GenerateEnumType.DescriptionValueAndDescriptionText);
        }

        public LocationManageModel(int? locationTypeId)
            : this()
        {
            if (locationTypeId.HasValue)
            {
                LocationTypeIds = new List<int>
                {
                    locationTypeId.Value
                };
                LocationTypes = _locationTypeService.GetLocationTypes(LocationTypeIds);
            }
        }

        public LocationManageModel(Location location)
            : this()
        {
            Id = location.Id;
            Name = location.Name;
            ContactName = location.ContactName;
            ContactTitle = location.ContactTitle;
            AddressLine1 = location.AddressLine1;
            AddressLine2 = location.AddressLine2;

            State = location.State;

            Postcode = location.Postcode;
            Suburb = location.Suburb;
            Country = location.Country;
            Latitude = location.Latitude;
            Longitude = location.Longitude;
            Phone = location.Phone;
            Fax = location.Fax;
            Email = location.Email;
            TrainingAffiliation = location.TrainingAffiliation;
            PinImage = location.PinImage;
            OpeningHoursWeekdays = location.OpeningHoursWeekdays;
            OpeningHoursSaturday = location.OpeningHoursSaturday;
            OpeningHoursSunday = location.OpeningHoursSunday;
            TimeZone = location.TimeZone;

            LocationTypeIds = location.LocationLocationTypes.Select(l => l.LocationTypeId).ToList();
            LocationTypes = _locationTypeService.GetLocationTypes(LocationTypeIds);
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [StringLength(255)]
        [LocalizedDisplayName("Location_Field_Name")]
        public string Name { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Location_Field_ContactName")]
        public string ContactName { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Location_Field_ContactTitle")]
        public string ContactTitle { get; set; }

        #region Address

        [StringLength(255)]
        [LocalizedDisplayName("Location_Field_AddressLine1")]
        public string AddressLine1 { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Location_Field_AddressLine2")]
        public string AddressLine2 { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Location_Field_Suburb")]
        public string Suburb { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Location_Field_State")]
        public string State { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Location_Field_Postcode")]
        public string Postcode { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Location_Field_Country")]
        public string Country { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        [LocalizedDisplayName("Location_Field_Latitude")]
        public double? Latitude { get; set; }

        [LocalizedDisplayName("Location_Field_Longitude")]
        public double? Longitude { get; set; }

        #endregion

        #region Phone

        [StringLength(50)]
        [LocalizedDisplayName("Location_Field_Phone")]
        public string Phone { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Location_Field_Fax")]
        public string Fax { get; set; }

        [StringLength(255)]
        [EmailValidation]
        [LocalizedDisplayName("Location_Field_Email")]
        public string Email { get; set; }

        #endregion

        [StringLength(255)]
        [LocalizedDisplayName("Location_Field_TrainingAffiliation")]
        public string TrainingAffiliation { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Location_Field_PinImage")]
        public string PinImage { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Location_Field_OpeningHoursWeekdays")]
        public string OpeningHoursWeekdays { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Location_Field_OpeningHoursSaturday")]
        public string OpeningHoursSaturday { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Location_Field_OpeningHoursSunday")]
        public string OpeningHoursSunday { get; set; }

        [LocalizedDisplayName("Location_Field_TimeZone")]
        public string TimeZone { get; set; }

        public IEnumerable<SelectListItem> TimeZones { get; set; }

        [LocalizedDisplayName("Location_Field_LocationTypeIds")]
        public List<int> LocationTypeIds { get; set; }

        public IEnumerable<SelectListItem> LocationTypes { get; set; }

        #endregion
    }
}
