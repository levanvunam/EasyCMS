using Ez.Framework.Core.Attributes;
using EzCMS.Entity.Entities.Models;
using System;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.AssociateTypes
{
    public class AssociateTypeDetailModel
    {
        public AssociateTypeDetailModel()
        {
        }

        public AssociateTypeDetailModel(AssociateType associateType)
            : this()
        {
            Id = associateType.Id;

            AssociateType = new AssociateTypeManageModel(associateType);

            Created = associateType.Created;
            CreatedBy = associateType.CreatedBy;
            LastUpdate = associateType.LastUpdate;
            LastUpdateBy = associateType.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("AssociateType_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("AssociateType_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("AssociateType_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("AssociateType_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public AssociateTypeManageModel AssociateType { get; set; }

        #endregion
    }
}
