using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.UserGroups
{
    public class UserGroupDetailModel
    {
        public UserGroupDetailModel()
        {
        }

        public UserGroupDetailModel(UserGroup userGroup)
            : this()
        {
            Id = userGroup.Id;
            ToolbarName = userGroup.ToolbarId.HasValue ? userGroup.Toolbar.Name : string.Empty;

            UserGroup = new UserGroupManageModel(userGroup);

            Created = userGroup.Created;
            CreatedBy = userGroup.CreatedBy;
            LastUpdate = userGroup.LastUpdate;
            LastUpdateBy = userGroup.LastUpdateBy;

        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("UserGroup_Field_ToolbarName")]
        public string ToolbarName { get; set; }

        [LocalizedDisplayName("UserGroup_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("UserGroup_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("UserGroup_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("UserGroup_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public UserGroupManageModel UserGroup { get; set; }

        #endregion
    }
}
