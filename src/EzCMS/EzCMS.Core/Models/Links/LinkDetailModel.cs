using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Links
{
    public class LinkDetailModel
    {
        public LinkDetailModel()
        {
        }

        public LinkDetailModel(Link link)
            : this()
        {
            Id = link.Id;

            Link = new LinkManageModel(link);

            Created = link.Created;
            CreatedBy = link.CreatedBy;
            LastUpdate = link.LastUpdate;
            LastUpdateBy = link.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("Link_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("Link_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("Link_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("Link_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public LinkManageModel Link { get; set; }

        #endregion
    }
}
