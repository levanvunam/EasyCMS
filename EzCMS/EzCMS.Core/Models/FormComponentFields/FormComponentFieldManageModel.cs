using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Shared.Forms;
using EzCMS.Core.Services.FormComponents;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.FormComponentFields
{
    public class FormComponentFieldManageModel
    {
        private readonly IFormComponentService _formComponentService;

        #region Constructors

        public FormComponentFieldManageModel()
        {
            _formComponentService = HostContainer.GetInstance<IFormComponentService>();

            FormComponents = _formComponentService.GetFormComponents();
            Attributes = new FormComponentFieldAttributeModel();
        }

        public FormComponentFieldManageModel(FormComponentField formComponentField)
            : this()
        {
            Id = formComponentField.Id;
            Name = formComponentField.Name;
            Attributes = SerializeUtilities.Deserialize<FormComponentFieldAttributeModel>(formComponentField.Attributes);

            FormComponentId = formComponentField.FormComponentId;
            FormComponents = _formComponentService.GetFormComponents(formComponentField.FormComponentId);

            RecordOrder = formComponentField.RecordOrder;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [StringLength(60)]
        [LocalizedDisplayName("FormComponentField_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("FormComponentField_Field_Attributes")]
        public FormComponentFieldAttributeModel Attributes { get; set; }

        [LocalizedDisplayName("FormComponentField_Field_FormComponentId")]
        public int FormComponentId { get; set; }

        public IEnumerable<SelectListItem> FormComponents { get; set; }

        [LocalizedDisplayName("FormComponentField_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion
    }
}
