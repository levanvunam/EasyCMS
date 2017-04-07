using System;
using System.Collections.Generic;
using Ez.Framework.Models;

namespace EzCMS.Core.Models.ClientNavigations
{
    public class ClientNavigationModel : BaseGridModel
    {
        #region Public Properties

        public int? PageId { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string UrlTarget { get; set; }

        public bool IsPageNavigation { get; set; }

        public List<int> ViewableGroupIds { get; set; }

        public List<int> EditableGroupIds { get; set; }

        public bool IncludeInSiteNavigation { get; set; }

        public bool DisableNavigationCascade { get; set; }

        public DateTime? StartPublishingDate { get; set; }

        public DateTime? EndPublishingDate { get; set; }

        public int? ParentId { get; set; }

        public string ParentName { get; set; }

        public string Hierarchy { get; set; }

        public bool Visible { get; set; }

        #endregion
    }
}
