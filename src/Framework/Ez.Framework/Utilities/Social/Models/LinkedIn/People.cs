using System;
using System.Xml.Serialization;

namespace Ez.Framework.Utilities.Social.Models.LinkedIn
{
    [Serializable]
    public class People
    {
        [XmlElement("person")]
        public Person[] Persons { get; set; }

        [XmlAttribute("total")]
        public int Total { get; set; }

        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlAttribute("start")]
        public int Start { get; set; }
    }
}