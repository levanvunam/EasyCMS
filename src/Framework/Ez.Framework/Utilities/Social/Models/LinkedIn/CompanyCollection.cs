using System;
using System.Xml.Serialization;

namespace Ez.Framework.Utilities.Social.Models.LinkedIn
{
    [Serializable, XmlRoot("companies")]
    public class CompanyCollection
    {
        [XmlElement("company")]
        public Company[] Companies { get; set; }
    }
}