using System;
using System.Collections.Generic;

namespace EzCMS.Core.Models.RssFeeds.Widgets
{
    public class RssItemWidget
    {
        #region Public Properties

        public string Title { get; set; }

        public string Link { get; set; }

        public string Comments { get; set; }

        /// <summary>
        /// Wordpress feed is Creator
        /// </summary>
        public string Author { get; set; }

        public DateTime? PubDate { get; set; }

        /// <summary>
        /// Google blog only
        /// </summary>
        public DateTime? Updated { get; set; }

        public List<string> Categories { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Featurename field in google blog
        /// </summary>
        public string Address { get; set; }

        public string Guid { get; set; }

        public List<MediaContent> Media { get; set; } 

        #endregion
    }

    public class MediaContent
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Player { get; set; }

        public string Thumbnail { get; set; }
    }
}
