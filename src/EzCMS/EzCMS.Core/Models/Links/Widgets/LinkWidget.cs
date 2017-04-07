using System;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Links.Widgets
{
    public class LinkWidget
    {
        public LinkWidget()
        {

        }

        public LinkWidget(Link link)
            : this()
        {
            Name = link.Name;
            Url = link.Url;
            UrlTarget = link.UrlTarget;
            Description = link.Description;
            LinkTypeId = link.LinkTypeId;
            LinkType = link.LinkType.Name;
            DateStart = link.DateStart;
            DateEnd = link.DateEnd;
        }

        #region Public Properties

        public string Name { get; set; }

        public string Url { get; set; }

        public string UrlTarget { get; set; }

        public string Description { get; set; }

        public int LinkTypeId { get; set; }

        public string LinkType { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        #endregion
    }
}
