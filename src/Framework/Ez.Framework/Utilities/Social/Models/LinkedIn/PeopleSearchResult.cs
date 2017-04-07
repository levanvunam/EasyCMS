using System;
using System.Xml.Serialization;

namespace Ez.Framework.Utilities.Social.Models.LinkedIn
{
    [Serializable, XmlRoot("people-search")]
    public class PeopleSearchResult
    {
        [XmlElement("people")]
        public People People { get; set; }

        [XmlElement("num-results")]
        public int Count { get; set; }
    }
}
