using System.Xml.Serialization;

namespace Ez.Framework.Utilities.Social.Models.LinkedIn
{
    public class Company
    {
        [XmlElement("id")]
        public int Id { get; set; }

        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }
    }
}