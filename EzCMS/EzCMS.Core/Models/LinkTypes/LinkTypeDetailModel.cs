using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.LinkTypes
{
    public class LinkTypeDetailModel
    {
        public LinkTypeDetailModel()
        {
        }

        public LinkTypeDetailModel(LinkType linkType)
            : this()
        {
            Id = linkType.Id;

            LinkType = new LinkTypeManageModel(linkType);

            Created = linkType.Created;
            CreatedBy = linkType.CreatedBy;
            LastUpdate = linkType.LastUpdate;
            LastUpdateBy = linkType.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("LinkType_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("LinkType_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("LinkType_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("LinkType_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public LinkTypeManageModel LinkType { get; set; }

        #endregion
    }
}
