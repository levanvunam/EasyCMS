using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.FormDefaultComponents
{
    public class FormDefaultComponentDetailModel
    {
        #region Constructors

        public FormDefaultComponentDetailModel()
        {
        }

        public FormDefaultComponentDetailModel(FormDefaultComponent formDefaultComponent)
            : this()
        {
            Id = formDefaultComponent.Id;

            FormDefaultComponent = new FormDefaultComponentManageModel(formDefaultComponent);

            FormComponentTemplateName = formDefaultComponent.FormComponentTemplate.Name;

            Created = formDefaultComponent.Created;
            CreatedBy = formDefaultComponent.CreatedBy;
            LastUpdate = formDefaultComponent.LastUpdate;
            LastUpdateBy = formDefaultComponent.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("FormDefaultComponent_Field_FormComponentTemplateName")]
        public string FormComponentTemplateName { get; set; }

        [LocalizedDisplayName("FormDefaultComponent_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("FormDefaultComponent_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("FormDefaultComponent_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("FormDefaultComponent_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public FormDefaultComponentManageModel FormDefaultComponent { get; set; }

        #endregion
    }
}
