using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.FormTabs
{
    public class FormTabManageModel
    {
        #region Constructors

        public FormTabManageModel()
        {
        }

        public FormTabManageModel(FormTab formTab)
            : this()
        {
            Id = formTab.Id;
            Name = formTab.Name;
            RecordOrder = formTab.RecordOrder;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [StringLength(60)]
        [LocalizedDisplayName("FormTab_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("FormTab_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion
    }
}
