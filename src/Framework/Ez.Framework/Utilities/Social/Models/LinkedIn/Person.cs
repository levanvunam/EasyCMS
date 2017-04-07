using System;
using System.Xml.Serialization;

namespace Ez.Framework.Utilities.Social.Models.LinkedIn
{
    [Serializable, XmlRoot("person")]
    public class Person
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("first-name")]
        public string FirstName { get; set; }

        [XmlElement("last-name")]
        public string LastName { get; set; }

        [XmlElement("headline")]
        public string HeadLine { get; set; }

        [XmlElement("url")]
        public string ProfileUrl { get; set; }

        [XmlElement("email_address")]
        public string Email { get; set; }
    }
}