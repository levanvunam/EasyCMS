using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.FormTabs
{
    public class FormTabDetailModel
    {
        #region Constructors

        public FormTabDetailModel()
        {
        }

        public FormTabDetailModel(FormTab formTab)
            : this()
        {
            Id = formTab.Id;

            FormTab = new FormTabManageModel(formTab);

            Created = formTab.Created;
            CreatedBy = formTab.CreatedBy;
            LastUpdate = formTab.LastUpdate;
            LastUpdateBy = formTab.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("FormTab_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("FormTab_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("FormTab_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("FormTab_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public FormTabManageModel FormTab { get; set; }

        #endregion
    }
}
