using Ez.Framework.Models;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.FormTabs
{
    public class FormTabModel : BaseGridModel
    {
        public FormTabModel()
        {

        }

        public FormTabModel(FormTab formTab)
            : base(formTab)
        {
            Name = formTab.Name;

            RecordOrder = formTab.RecordOrder;
            Created = formTab.Created;
            CreatedBy = formTab.CreatedBy;
            LastUpdate = formTab.LastUpdate;
            LastUpdateBy = formTab.LastUpdateBy;
        }

        #region Public Properties

        public string Name { get; set; }

        #endregion
    }
}
