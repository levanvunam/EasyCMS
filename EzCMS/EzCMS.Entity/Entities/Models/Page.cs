using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Entity.Entities.Models
{
    public class Page : BaseHierachyModel<Page>
    {
        [StringLength(255)]
        public string Title { get; set; }

        public string Abstract { get; set; }

        [StringLength(255)]
        public string SeoTitle { get; set; }

        public string Description { get; set; }

        public string AbstractWorking { get; set; }

        public string Content { get; set; }

        public string ContentWorking { get; set; }

        [StringLength(255)]
        public string FriendlyUrl { get; set; }

        [StringLength(255)]
        public string Keywords { get; set; }

        public int? PageTemplateId { get; set; }

        [ForeignKey("PageTemplateId")]
        public virtual PageTemplate PageTemplate { get; set; }

        public int? FileTemplateId { get; set; }

        [ForeignKey("FileTemplateId")]
        public virtual FileTemplate FileTemplate { get; set; }

        public int? BodyTemplateId { get; set; }

        [ForeignKey("BodyTemplateId")]
        public virtual BodyTemplate BodyTemplate { get; set; }

        public PageEnums.PageStatus Status { get; set; }

        public bool IsHomePage { get; set; }

        public bool IncludeInSiteNavigation { get; set; }

        public bool DisableNavigationCascade { get; set; }

        public DateTime? StartPublishingDate { get; set; }

        public DateTime? EndPublishingDate { get; set; }

        public bool SSL { get; set; }

        public virtual ICollection<ClientNavigation> ClientNavigations { get; set; }

        public virtual ICollection<PageLog> PageLogs { get; set; }

        public virtual ICollection<Page> ChildrenPages { get; set; }

        public virtual ICollection<PageSecurity> PageSecurities { get; set; }

        public virtual ICollection<PageTag> PageTags { get; set; }

        public virtual ICollection<LinkTracker> LinkTrackers { get; set; }

        public virtual ICollection<PageRead> PageReads { get; set; }
    }
}