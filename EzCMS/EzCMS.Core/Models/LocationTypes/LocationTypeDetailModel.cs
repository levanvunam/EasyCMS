using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.LocationTypes
{
    public class LocationTypeDetailModel
    {
        public LocationTypeDetailModel()
        {
        }

        public LocationTypeDetailModel(LocationType associateType)
            : this()
        {
            Id = associateType.Id;

            LocationType = new LocationTypeManageModel(associateType);

            Created = associateType.Created;
            CreatedBy = associateType.CreatedBy;
            LastUpdate = associateType.LastUpdate;
            LastUpdateBy = associateType.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("LocationType_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("LocationType_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("LocationType_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("LocationType_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public LocationTypeManageModel LocationType { get; set; }

        #endregion
    }
}
