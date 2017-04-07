using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Pages
{
    public class PageDetailModel
    {
        public PageDetailModel()
        {
        }

        public PageDetailModel(Entity.Entities.Models.Page page)
            : this()
        {
            Id = page.Id;

            Page = new PageManageModel(page);
            IsHomePage = page.IsHomePage;
            ParentName = page.Parent != null ? page.Parent.Title : string.Empty;
            PageTemplateName = page.PageTemplate != null ? page.PageTemplate.Name : string.Empty;
            FileTemplateName = page.FileTemplate != null ? page.FileTemplate.Name : string.Empty;
            BodyTemplateName = page.BodyTemplate != null ? page.BodyTemplate.Name : string.Empty;

            RecordOrder = page.RecordOrder;
            Created = page.Created;
            CreatedBy = page.CreatedBy;
            LastUpdate = page.LastUpdate;
            LastUpdateBy = page.LastUpdateBy;

        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("Page_Field_PageTemplateName")]
        public string PageTemplateName { get; set; }

        [LocalizedDisplayName("Page_Field_FileTemplateName")]
        public string FileTemplateName { get; set; }

        [LocalizedDisplayName("Page_Field_BodyTemplateName")]
        public string BodyTemplateName { get; set; }

        public bool IsHomePage { get; set; }

        [LocalizedDisplayName("Page_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        [LocalizedDisplayName("Page_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("Page_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("Page_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("Page_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        [LocalizedDisplayName("Page_Field_ParentName")]
        public string ParentName { get; set; }
        
        public PageManageModel Page { get; set; }

        #endregion
    }
}
