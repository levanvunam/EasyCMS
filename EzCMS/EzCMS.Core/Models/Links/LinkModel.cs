using System;
using Ez.Framework.Models;

namespace EzCMS.Core.Models.Links
{
    public class LinkModel : BaseGridModel
    {
        #region Public Properties

        public string Name { get; set; }

        public string Url { get; set; }

        public string UrlTarget { get; set; }

        public string Description { get; set; }

        public int LinkTypeId { get; set; }

        public string LinkTypeName { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        #endregion
    }
}
