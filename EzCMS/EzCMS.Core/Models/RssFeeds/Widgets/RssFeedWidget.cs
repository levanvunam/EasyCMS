using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.RssFeeds.Widgets
{
    public class RssFeedWidget
    {
        public RssFeedWidget()
        {
            Items = new List<RssItemWidget>();
        }

        public RssFeedWidget(RssFeed rssFeed, int? number)
            : this()
        {
            Id = rssFeed.Id;
            Name = rssFeed.Name;
            Url = rssFeed.Url;
            RssType = rssFeed.RssType;

            #region Load RSS

            var rssXmlDoc = new XmlDocument();
            rssXmlDoc.Load(Url);

            var reader = XmlReader.Create(Url);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();

            if (feed != null)
            {
                var items = feed.Items;
                if (number.HasValue && number.Value > 0)
                    items = items.Skip(0).Take(number.Value);

                foreach (var item in items)
                {
                    var rssItemWidget = new RssItemWidget
                    {
                        Title = item.Title != null ? item.Title.Text : string.Empty,
                        Link = item.Links.Any() ? item.Links.ElementAt(0).Uri.AbsoluteUri : string.Empty,
                        Author = item.Authors.Any() ? item.Authors.ElementAt(0).Name : string.Empty,
                        PubDate = item.PublishDate.UtcDateTime,
                        Updated = item.LastUpdatedTime.UtcDateTime,
                        Categories = item.Categories.Select(category => category.Name).ToList(),
                        Description = item.Summary != null ? item.Summary.Text : string.Empty,

                        // Extension elements
                        Comments = GetExtensionElement("Comments", item),
                        Address = GetExtensionElement("featurename", item),
                        Guid = GetExtensionElement("guid", item)
                    };
                    Items.Add(rssItemWidget);
                }
            }

            #endregion
        }

        #region Methods

        /// <summary>
        /// Get single extension element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetExtensionElement(string element, SyndicationItem item)
        {
            foreach (SyndicationElementExtension extension in item.ElementExtensions)
            {
                if (extension.OuterName.Equals(element, StringComparison.CurrentCultureIgnoreCase))
                {
                    return extension.GetObject<string>();
                }
            }
            return string.Empty;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public RssFeedEnums.RssType RssType { get; set; }

        public List<RssItemWidget> Items { get; set; }

        #endregion
    }
}
