using Ez.Framework.Core.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzCMS.Entity.Entities.Models
{
    public class ClientNavigation : BaseHierachyModel<ClientNavigation>
    {
        [StringLength(512)]
        public string Title { get; set; }

        public int? PageId { get; set; }

        [ForeignKey("PageId")]
        public virtual Page Page { get; set; }

        [StringLength(512)]
        public string Url { get; set; }

        public string UrlTarget { get; set; }

        public bool IncludeInSiteNavigation { get; set; }

        public bool DisableNavigationCascade { get; set; }

        public DateTime? StartPublishingDate { get; set; }

        public DateTime? EndPublishingDate { get; set; }

        public virtual ICollection<ClientNavigation> ChildNavigations { get; set; }
    }
}