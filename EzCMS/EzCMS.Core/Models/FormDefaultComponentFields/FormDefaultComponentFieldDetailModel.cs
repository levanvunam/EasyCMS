using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.FormDefaultComponentFields
{
    public class FormDefaultComponentFieldDetailModel
    {
        #region Constructors

        public FormDefaultComponentFieldDetailModel()
        {
        }

        public FormDefaultComponentFieldDetailModel(FormDefaultComponentField formDefaultComponentField)
            : this()
        {
            Id = formDefaultComponentField.Id;

            FormDefaultComponentField = new FormDefaultComponentFieldManageModel(formDefaultComponentField);

            FormDefaultComponentName = formDefaultComponentField.FormDefaultComponent.Name;
            Attributes = formDefaultComponentField.Attributes;

            Created = formDefaultComponentField.Created;
            CreatedBy = formDefaultComponentField.CreatedBy;
            LastUpdate = formDefaultComponentField.LastUpdate;
            LastUpdateBy = formDefaultComponentField.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("FormDefaultComponentField_Field_Attributes")]
        public string Attributes { get; set; }

        [LocalizedDisplayName("FormDefaultComponentField_Field_FormDefaultComponentName")]
        public string FormDefaultComponentName { get; set; }

        [LocalizedDisplayName("FormDefaultComponentField_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("FormDefaultComponentField_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("FormDefaultComponentField_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("FormDefaultComponentField_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public FormDefaultComponentFieldManageModel FormDefaultComponentField { get; set; }

        #endregion
    }
}
