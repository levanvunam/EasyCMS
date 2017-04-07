using System;
using Ez.Framework.Models;
using Ez.Framework.Core.Attributes;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.PageLogs
{
    public class PageLogModel : BaseGridModel
    {
        #region Constructors
        public PageLogModel()
        {

        }

        public PageLogModel(PageLog pageLog)
        {
            PageId = pageLog.Id;
            Title = pageLog.Title;
            PageTemplateId = pageLog.Page.PageTemplateId;
            FileTemplateId = pageLog.FileTemplateId;
            BodyTemplateId = pageLog.BodyTemplateId;
            Status = pageLog.Status;
            FriendlyUrl = pageLog.FriendlyUrl;
            ParentId = pageLog.ParentId;
            SSL = pageLog.SSL;
        }

        public PageLogModel(Page page)
        {
            PageId = page.Id;
            Title = page.Title;
            FileTemplateId = page.FileTemplateId;
            PageTemplateId = page.PageTemplateId;
            BodyTemplateId = page.BodyTemplateId;
            SSL = page.SSL;
            Status = page.Status;
            FriendlyUrl = page.FriendlyUrl;
            ParentId = page.ParentId;
        }
        #endregion

        #region Public Properties

        public int PageId { get; set; }

        public string PageTitle { get; set; }

        public string Title { get; set; }

        public string ChangeLog { get; set; }

        public int? FileTemplateId { get; set; }

        public int? PageTemplateId { get; set; }

        public int? BodyTemplateId { get; set; }

        public PageEnums.PageStatus Status { get; set; }

        public bool IsHomePage { get; set; }

        public bool SSL { get; set; }

        public string FriendlyUrl { get; set; }

        public int? ParentId { get; set; }

        [DefaultOrder]
        public new DateTime? Created { get; set; }

        #endregion
    }
}
