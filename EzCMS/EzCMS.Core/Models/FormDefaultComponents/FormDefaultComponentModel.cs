using Ez.Framework.Models;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.FormDefaultComponents
{
    public class FormDefaultComponentModel : BaseGridModel
    {
        public FormDefaultComponentModel()
        {

        }

        public FormDefaultComponentModel(FormDefaultComponent formDefaultComponent)
            : base(formDefaultComponent)
        {
            Name = formDefaultComponent.Name;
            FormComponentTemplateId = formDefaultComponent.FormComponentTemplateId;
            FormComponentTemplateName = formDefaultComponent.FormComponentTemplate.Name;
        }

        #region Public Properties

        public string Name { get; set; }

        public int FormComponentTemplateId { get; set; }

        public string FormComponentTemplateName { get; set; }

        #endregion
    }
}
