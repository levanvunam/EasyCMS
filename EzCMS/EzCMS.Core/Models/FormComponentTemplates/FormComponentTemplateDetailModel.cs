using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.FormComponentTemplates
{
    public class FormComponentTemplateDetailModel
    {
        #region Constructors

        public FormComponentTemplateDetailModel()
        {
        }

        public FormComponentTemplateDetailModel(FormComponentTemplate formComponentTemplate)
            : this()
        {
            Id = formComponentTemplate.Id;

            FormComponentTemplate = new FormComponentTemplateManageModel(formComponentTemplate);

            Created = formComponentTemplate.Created;
            CreatedBy = formComponentTemplate.CreatedBy;
            LastUpdate = formComponentTemplate.LastUpdate;
            LastUpdateBy = formComponentTemplate.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("FormComponentTemplate_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("FormComponentTemplate_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("FormComponentTemplate_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("FormComponentTemplate_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public FormComponentTemplateManageModel FormComponentTemplate { get; set; }

        #endregion
    }
}
