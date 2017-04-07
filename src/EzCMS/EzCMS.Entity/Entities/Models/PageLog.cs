using System;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Entity.Entities.Models
{
    public class PageLog : BaseModel
    {
        [StringLength(512)]
        public string SessionId { get; set; }

        public int PageId { get; set; }

        public int? ParentId { get; set; }

        [StringLength(512)]
        public string Title { get; set; }

        public string SeoTitle { get; set; }

        public string Description { get; set; }

        public string Abstract { get; set; }

        public string AbstractWorking { get; set; }

        public string Content { get; set; }

        public string ContentWorking { get; set; }

        [StringLength(512)]
        public string FriendlyUrl { get; set; }

        public int? PageTemplateId { get; set; }

        public int? FileTemplateId { get; set; }

        public int? BodyTemplateId { get; set; }

        public PageEnums.PageStatus Status { get; set; }

        public bool IncludeInSiteNavigation { get; set; }

        public bool DisableNavigationCascade { get; set; }

        public DateTime? StartPublishingDate { get; set; }

        public DateTime? EndPublishingDate { get; set; }

        public bool SSL { get; set; }

        [StringLength(512)]
        public string Keywords { get; set; }

        public string ChangeLog { get; set; }

        public virtual Page Page { get; set; }
    }
}