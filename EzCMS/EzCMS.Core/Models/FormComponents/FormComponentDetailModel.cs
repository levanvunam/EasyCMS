using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.FormComponents
{
    public class FormComponentDetailModel
    {
        #region Constructors

        public FormComponentDetailModel()
        {
        }

        public FormComponentDetailModel(FormComponent formComponent)
            : this()
        {
            Id = formComponent.Id;

            FormComponent = new FormComponentManageModel(formComponent);

            FormTabName = formComponent.FormTab.Name;
            FormComponentTemplateName = formComponent.FormComponentTemplate.Name;

            Created = formComponent.Created;
            CreatedBy = formComponent.CreatedBy;
            LastUpdate = formComponent.LastUpdate;
            LastUpdateBy = formComponent.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("FormComponent_Field_FormTabName")]
        public string FormTabName { get; set; }

        [LocalizedDisplayName("FormComponent_Field_FormComponentTemplateName")]
        public string FormComponentTemplateName { get; set; }

        [LocalizedDisplayName("FormComponent_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("FormComponent_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("FormComponent_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("FormComponent_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public FormComponentManageModel FormComponent { get; set; }

        #endregion
    }
}
