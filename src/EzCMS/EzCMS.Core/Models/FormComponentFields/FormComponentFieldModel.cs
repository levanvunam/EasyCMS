using Ez.Framework.Models;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.FormComponentFields
{
    public class FormComponentFieldModel : BaseGridModel
    {
        public FormComponentFieldModel()
        {

        }

        public FormComponentFieldModel(FormComponentField formComponentField)
            : this()
        {
            Id = formComponentField.Id;
            Name = formComponentField.Name;
            Attributes = formComponentField.Attributes;

            FormComponentId = formComponentField.FormComponentId;
            FormComponentName = formComponentField.FormComponent.Name;

            RecordOrder = formComponentField.RecordOrder;
            Created = formComponentField.Created;
            CreatedBy = formComponentField.CreatedBy;
            LastUpdate = formComponentField.LastUpdate;
            LastUpdateBy = formComponentField.LastUpdateBy;
        }

        #region Public Properties

        public string Name { get; set; }

        public string Attributes { get; set; }

        public int FormComponentId { get; set; }

        public string FormComponentName { get; set; }

        #endregion
    }
}
