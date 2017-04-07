using Ez.Framework.Models;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.FormDefaultComponentFields
{
    public class FormDefaultComponentFieldModel : BaseGridModel
    {
        public FormDefaultComponentFieldModel()
        {

        }

        public FormDefaultComponentFieldModel(FormDefaultComponentField formDefaultComponentField)
            : this()
        {
            Id = formDefaultComponentField.Id;
            Name = formDefaultComponentField.Name;
            Attributes = formDefaultComponentField.Attributes;

            FormDefaultComponentId = formDefaultComponentField.FormDefaultComponentId;
            FormDefaultComponentName = formDefaultComponentField.FormDefaultComponent.Name;

            RecordOrder = formDefaultComponentField.RecordOrder;
            Created = formDefaultComponentField.Created;
            CreatedBy = formDefaultComponentField.CreatedBy;
            LastUpdate = formDefaultComponentField.LastUpdate;
            LastUpdateBy = formDefaultComponentField.LastUpdateBy;
        }

        #region Public Properties

        public string Name { get; set; }

        public string Attributes { get; set; }

        public int FormDefaultComponentId { get; set; }

        public string FormDefaultComponentName { get; set; }

        #endregion
    }
}
