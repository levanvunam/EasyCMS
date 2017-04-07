using System;
using System.Xml.Serialization;

namespace Ez.Framework.Utilities.Social.Models.LinkedIn
{
    [Serializable, XmlRoot("update")]
    public class LinkedInShareResponse
    {
        [XmlElement("update-key")]
        public string UpdateKey { get; set; }

        [XmlElement("update-url")]
        public string UpdateUrl { get; set; }
    }

    [Serializable]
    public class LinkedInPostResultResponse
    {
        [XmlElement("errorCode")]
        public string ErrorCode { get; set; }

        [XmlElement("message")]
        public string Message { get; set; }

        [XmlElement("requestId")]
        public string RequestId { get; set; }

        [XmlElement("status")]
        public string Status { get; set; }

        [XmlElement("timestamp")]
        public string Timestamp { get; set; }
    }
}