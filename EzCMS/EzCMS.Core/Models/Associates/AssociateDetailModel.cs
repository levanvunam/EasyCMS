using Ez.Framework.Core.Attributes;
using EzCMS.Entity.Entities.Models;
using System;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Associates
{
    public class AssociateDetailModel
    {
        public AssociateDetailModel()
        {

        }

        public AssociateDetailModel(Associate item)
            : this()
        {
            Id = item.Id;

            Associate = new AssociateManageModel(item);

            Created = item.Created;
            CreatedBy = item.CreatedBy;
            LastUpdate = item.LastUpdate;
            LastUpdateBy = item.LastUpdateBy;
        }

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("Associate_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("Associate_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("Associate_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("Associate_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public AssociateManageModel Associate { get; set; }

        #endregion
    }
}
