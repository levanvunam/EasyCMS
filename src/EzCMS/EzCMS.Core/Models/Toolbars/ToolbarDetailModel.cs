using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Toolbars
{
    public class ToolbarDetailModel
    {
        public ToolbarDetailModel()
        {
        }

        public ToolbarDetailModel(Toolbar toolbar)
            : this()
        {
            Id = toolbar.Id;
            IsDefault = toolbar.IsDefault;

            Toolbar = new ToolbarManageModel(toolbar);

            Created = toolbar.Created;
            CreatedBy = toolbar.CreatedBy;
            LastUpdate = toolbar.LastUpdate;
            LastUpdateBy = toolbar.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("Toolbar_Field_IsDefault")]
        public bool IsDefault { get; set; }

        [LocalizedDisplayName("Toolbar_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("Toolbar_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("Toolbar_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("Toolbar_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public ToolbarManageModel Toolbar { get; set; }

        #endregion
    }
}
