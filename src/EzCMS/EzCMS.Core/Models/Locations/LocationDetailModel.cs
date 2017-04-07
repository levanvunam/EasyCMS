using System;
using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Locations
{
    public class LocationDetailModel
    {
        public LocationDetailModel()
        {
        }

        public LocationDetailModel(Location location)
            : this()
        {
            Id = location.Id;
            LocationTypes = location.LocationLocationTypes.Select(l => l.LocationType.Name).ToList();
            Location = new LocationManageModel(location);

            Created = location.Created;
            CreatedBy = location.CreatedBy;
            LastUpdate = location.LastUpdate;
            LastUpdateBy = location.LastUpdateBy;

        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("Location_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("Location_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("Location_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("Location_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public List<string> LocationTypes { get; set; }

        public LocationManageModel Location { get; set; }

        #endregion
    }
}
