using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.FormComponentFields
{
    public class FormComponentFieldDetailModel
    {
        #region Constructors

        public FormComponentFieldDetailModel()
        {
        }

        public FormComponentFieldDetailModel(FormComponentField formComponentField)
            : this()
        {
            Id = formComponentField.Id;

            FormComponentField = new FormComponentFieldManageModel(formComponentField);

            FormComponentName = formComponentField.FormComponent.Name;
            Attributes = formComponentField.Attributes;

            Created = formComponentField.Created;
            CreatedBy = formComponentField.CreatedBy;
            LastUpdate = formComponentField.LastUpdate;
            LastUpdateBy = formComponentField.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("FormComponentField_Field_Attributes")]
        public string Attributes { get; set; }

        [LocalizedDisplayName("FormComponentField_Field_FormComponentName")]
        public string FormComponentName { get; set; }

        [LocalizedDisplayName("FormComponentField_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("FormComponentField_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("FormComponentField_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("FormComponentField_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public FormComponentFieldManageModel FormComponentField { get; set; }

        #endregion
    }
}
