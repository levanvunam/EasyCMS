using Ez.Framework.Models;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.FormComponents
{
    public class FormComponentModel : BaseGridModel
    {
        public FormComponentModel()
        {

        }

        public FormComponentModel(FormComponent formComponent)
            : this()
        {
            Name = formComponent.Name;
            FormComponentTemplateId = formComponent.FormComponentTemplateId;
            FormComponentTemplateName = formComponent.FormComponentTemplate.Name;

            RecordOrder = formComponent.RecordOrder;
            Created = formComponent.Created;
            CreatedBy = formComponent.CreatedBy;
            LastUpdate = formComponent.LastUpdate;
            LastUpdateBy = formComponent.LastUpdateBy;
        }

        #region Public Properties

        public string Name { get; set; }

        public int FormComponentTemplateId { get; set; }

        public string FormComponentTemplateName { get; set; }

        #endregion
    }
}
