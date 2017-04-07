using Ez.Framework.Core.Attributes;
using Ez.Framework.Models;
using Ez.Framework.Utilities.Reflection.Attributes;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Pages
{
    public class PageModel : BaseGridModel
    {
        #region Constructors

        public PageModel()
        {
        }

        public PageModel(Page page)
            : base(page)
        {
            Id = page.Id;
            Title = page.Title;
            FriendlyUrl = page.FriendlyUrl;
            IsHomePage = page.IsHomePage;
            Status = page.Status;

            TotalReads = page.PageReads.Count;

            ParentId = page.ParentId;
            ParentName = page.ParentId.HasValue ? page.Parent.Title : string.Empty;
            Hierarchy = page.Hierarchy;

            PageTemplateId = page.PageTemplateId;
            PageTemplateName = page.PageTemplateId.HasValue ? page.PageTemplate.Name : string.Empty;
            FileTemplateId = page.FileTemplateId;
            FileTemplateName = page.FileTemplateId.HasValue ? page.FileTemplate.Name : string.Empty;
            BodyTemplateId = page.BodyTemplateId;
            BodyTemplateName = page.BodyTemplateId.HasValue ? page.BodyTemplate.Name : string.Empty;
        }

        #endregion

        #region Public Properties

        public string Title { get; set; }

        public PageEnums.PageStatus Status { get; set; }

        public bool IsHomePage { get; set; }

        public string FriendlyUrl { get; set; }

        public int TotalReads { get; set; }

        [IgnoreInDropdown]
        public int? PageTemplateId { get; set; }

        [IgnoreInDropdown]
        public string PageTemplateName { get; set; }

        [IgnoreInDropdown]
        public int? FileTemplateId { get; set; }

        [IgnoreInDropdown]
        public string FileTemplateName { get; set; }

        [IgnoreInDropdown]
        public int? BodyTemplateId { get; set; }

        [IgnoreInDropdown]
        public string BodyTemplateName { get; set; }

        [IgnoreInDropdown]
        public int? ParentId { get; set; }

        [IgnoreInDropdown]
        public string ParentName { get; set; }

        [DefaultOrder(Priority = OrderPriority.High)]
        public string Hierarchy { get; set; }

        #endregion
    }
}
