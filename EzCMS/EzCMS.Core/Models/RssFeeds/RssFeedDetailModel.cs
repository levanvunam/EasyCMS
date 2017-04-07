using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.RssFeeds
{
    public class RssFeedDetailModel
    {
        public RssFeedDetailModel()
        {

        }

        public RssFeedDetailModel(RssFeed rssFeed)
            : this()
        {
            RssFeed = new RssFeedManageModel(rssFeed);

            Id = rssFeed.Id;
            Created = rssFeed.Created;
            CreatedBy = rssFeed.CreatedBy;
            LastUpdate = rssFeed.LastUpdate;
            LastUpdateBy = rssFeed.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        public RssFeedManageModel RssFeed { get; set; }

        [LocalizedDisplayName("RSSFeed_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("RSSFeed_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("RSSFeed_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("RSSFeed_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        #endregion
    }
}
