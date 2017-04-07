using Ez.Framework.Core.Attributes;
using EzCMS.Core.Models.Shared;
using EzCMS.Entity.Entities.Models;
using System;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.BodyTemplates
{
    public class BodyTemplateDetailModel
    {
        public BodyTemplateDetailModel()
        {
        }

        public BodyTemplateDetailModel(BodyTemplate bodyTemplate)
            : this()
        {
            Id = bodyTemplate.Id;

            BodyTemplate = new BodyTemplateManageModel(bodyTemplate);

            ContentPreviewModel = new ContentPreviewModel(bodyTemplate.Content);

            Created = bodyTemplate.Created;
            CreatedBy = bodyTemplate.CreatedBy;
            LastUpdate = bodyTemplate.LastUpdate;
            LastUpdateBy = bodyTemplate.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("BodyTemplate_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("BodyTemplate_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("BodyTemplate_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("BodyTemplate_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public BodyTemplateManageModel BodyTemplate { get; set; }

        public ContentPreviewModel ContentPreviewModel { get; set; }

        #endregion
    }
}
