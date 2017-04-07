using System;
using System.Linq;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.PageLogs
{
    public class PageLogManageModel
    {
        #region Constructors

        public PageLogManageModel()
        {
            
        }

        public PageLogManageModel(Page page)
        {
            PageId = page.Id;
            Title = page.Title;
            SeoTitle = page.SeoTitle;
            FriendlyUrl = page.FriendlyUrl;
            Abstract = page.Abstract;
            AbstractWorking = page.AbstractWorking;
            Content = page.Content;
            ContentWorking = page.ContentWorking;

            var pageTags = page.PageTags.Select(t => t.Id);
            var tags = pageTags as int[] ?? pageTags.ToArray();
            Tags = tags.Any() ? string.Join(",", tags.ToList()) : string.Empty;

            FileTemplateId = page.FileTemplateId;
            PageTemplateId = page.PageTemplateId;
            BodyTemplateId = page.BodyTemplateId;
            ParentId = page.ParentId;

            Keywords = page.Keywords;
            IncludeInSiteNavigation = page.IncludeInSiteNavigation;
            DisableNavigationCascade = page.DisableNavigationCascade;
            StartPublishingDate = page.StartPublishingDate;
            EndPublishingDate = page.EndPublishingDate;
            Status = page.Status;
            SSL = page.SSL;
        }
        #endregion

        #region Public Properties
        public int Id { get; set; }

        public int PageId { get; set; }

        public string Title { get; set; }

        public string SeoTitle { get; set; }

        public string Description { get; set; }

        public string Abstract { get; set; }

        public string AbstractWorking { get; set; }

        public string Content { get; set; }

        public string ContentWorking { get; set; }

        public string Tags { get; set; }

        public string Keywords { get; set; }

        public bool IncludeInSiteNavigation { get; set; }

        public bool DisableNavigationCascade { get; set; }

        public DateTime? StartPublishingDate { get; set; }

        public DateTime? EndPublishingDate { get; set; }

        public int? FileTemplateId { get; set; }

        public int? PageTemplateId { get; set; }

        public int? BodyTemplateId { get; set; }

        public PageEnums.PageStatus Status { get; set; }

        public bool SSL { get; set; }

        public bool IsHomePage { get; set; }

        public string FriendlyUrl { get; set; }

        public int? ParentId { get; set; }

        #endregion
    }
}
