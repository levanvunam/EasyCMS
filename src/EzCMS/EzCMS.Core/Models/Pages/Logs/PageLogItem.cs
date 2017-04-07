using System;
using Ez.Framework.Core.Attributes;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Pages.Logs
{
    public class PageLogItem
    {
        #region Constructors

        public PageLogItem()
        {
        }

        public PageLogItem(PageLog pageLog): this()
        {
            Id = pageLog.Id;
            PageId = pageLog.PageId;
            SessionId = pageLog.SessionId;
            Title = pageLog.Title;
            ChangeLog = pageLog.ChangeLog;
            PageTemplateId = pageLog.Page.PageTemplateId;
            FileTemplateId = pageLog.FileTemplateId;
            BodyTemplateId = pageLog.BodyTemplateId;
            Status = pageLog.Status;
            FriendlyUrl = pageLog.FriendlyUrl;
            ParentId = pageLog.ParentId;
            Created = pageLog.Created;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public int PageId { get; set; }

        public string SessionId { get; set; }

        public string Title { get; set; }

        public string ChangeLog { get; set; }

        public int? FileTemplateId { get; set; }

        public int? PageTemplateId { get; set; }

        public int? BodyTemplateId { get; set; }

        public PageEnums.PageStatus Status { get; set; }

        public bool IsHomePage { get; set; }

        public string FriendlyUrl { get; set; }

        public int? ParentId { get; set; }

        [DefaultOrder]
        public DateTime? Created { get; set; }

        #endregion
    }
}
