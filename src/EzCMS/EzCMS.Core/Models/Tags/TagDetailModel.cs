using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Tags
{
    public class TagDetailModel
    {
        public TagDetailModel()
        {
        }

        public TagDetailModel(Tag tag)
            : this()
        {
            Id = tag.Id;

            Tag = new TagManageModel(tag);

            Created = tag.Created;
            CreatedBy = tag.CreatedBy;
            LastUpdate = tag.LastUpdate;
            LastUpdateBy = tag.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("Tag_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("Tag_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("Tag_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("Tag_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public TagManageModel Tag { get; set; }

        #endregion
    }
}
