using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.LinkTrackers
{
    public class LinkTrackerDetailModel
    {
        public LinkTrackerDetailModel()
        {

        }

        public LinkTrackerDetailModel(LinkTracker linkTracker)
            : this()
        {
            Id = linkTracker.Id;

            LinkTracker = new LinkTrackerManageModel(linkTracker);

            PageTitle = linkTracker.PageId.HasValue ? linkTracker.Page.Title : string.Empty;

            Created = linkTracker.Created;
            CreatedBy = linkTracker.CreatedBy;
            LastUpdate = linkTracker.LastUpdate;
            LastUpdateBy = linkTracker.LastUpdateBy;
        }

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("LinkTracker_Field_PageTitle")]
        public string PageTitle { get; set; }

        [LocalizedDisplayName("LinkTracker_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("LinkTracker_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("LinkTracker_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("LinkTracker_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public LinkTrackerManageModel LinkTracker { get; set; }

        #endregion
    }
}
