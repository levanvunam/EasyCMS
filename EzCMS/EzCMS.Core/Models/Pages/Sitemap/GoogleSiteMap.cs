using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EzCMS.Core.Models.Pages.Sitemap
{
    [XmlRoot("urlset")]
    public class GoogleSiteMap
    {
        [XmlAttribute("schemaLocation", Namespace = XmlSchema.InstanceNamespace)]
        public string XsiSchemaLocation =
            "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd";

        [XmlElement("url")]
        public List<GoogleSiteMapUrl> urls { get; set; }
    }
}