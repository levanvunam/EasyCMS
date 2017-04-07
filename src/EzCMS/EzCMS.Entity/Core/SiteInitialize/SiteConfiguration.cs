using System.Collections.Generic;
using System.Xml.Serialization;

namespace EzCMS.Entity.Core.SiteInitialize
{
    public class SiteConfiguration
    {
        public string ConnectionString { get; set; }

        public bool IsSetupFinish { get; set; }

        public List<Plugin> Plugins { get; set; }
    }

    public class Plugin
    {
        /// <summary>
        /// Plugin Name
        /// </summary>
        [XmlElement("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Plugin Name
        /// </summary>
        [XmlElement("Folder")]
        public string Folder { get; set; }

        /// <summary>
        /// Connection string
        /// </summary>
        [XmlElement("ConnectionString")]
        public string ConnectionString { get; set; }
    }
}