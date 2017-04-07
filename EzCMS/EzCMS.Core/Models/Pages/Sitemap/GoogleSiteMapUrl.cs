using System.Xml.Serialization;

namespace EzCMS.Core.Models.Pages.Sitemap
{
    public class GoogleSiteMapUrl
    {
        [XmlElement("loc")]
        public string Location { get; set; }

        [XmlElement("lastmod")]
        public string LastModified { get; set; }

        [XmlElement("changefreq")]
        public string ChangeFreq { get; set; }

        [XmlElement("priority")]
        public string Priority { get; set; }
    }
}