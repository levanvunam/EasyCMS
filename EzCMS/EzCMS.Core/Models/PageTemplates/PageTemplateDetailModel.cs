using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Shared;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.PageTemplates
{
    public class PageTemplateDetailModel
    {
        public PageTemplateDetailModel()
        {
        }

        public PageTemplateDetailModel(PageTemplate pageTemplate)
            : this()
        {
            Id = pageTemplate.Id;

            PageTemplate = new PageTemplateManageModel(pageTemplate);
            ParentName = pageTemplate.Parent != null ? pageTemplate.Parent.Name : string.Empty;

            ContentPreviewModel = new ContentPreviewModel(pageTemplate.Content);

            Created = pageTemplate.Created;
            CreatedBy = pageTemplate.CreatedBy;
            LastUpdate = pageTemplate.LastUpdate;
            LastUpdateBy = pageTemplate.LastUpdateBy;
        }

        #region Public Properties
        public int? Id { get; set; }

        [LocalizedDisplayName("PageTemplate_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("PageTemplate_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("PageTemplate_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("PageTemplate_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        [LocalizedDisplayName("PageTemplate_Field_ParentName")]
        public string ParentName { get; set; }

        public PageTemplateManageModel PageTemplate { get; set; }

        public ContentPreviewModel ContentPreviewModel { get; set; }


        #endregion

    }
}
