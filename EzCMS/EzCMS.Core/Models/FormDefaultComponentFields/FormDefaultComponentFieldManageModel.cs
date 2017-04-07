using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Shared.Forms;
using EzCMS.Core.Services.FormDefaultComponents;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.FormDefaultComponentFields
{
    public class FormDefaultComponentFieldManageModel
    {
        private readonly IFormDefaultComponentService _formDefaultComponentService;

        #region Constructors

        public FormDefaultComponentFieldManageModel()
        {
            _formDefaultComponentService = HostContainer.GetInstance<IFormDefaultComponentService>();

            FormDefaultComponents = _formDefaultComponentService.GetFormDefaultComponents();
            Attributes = new FormComponentFieldAttributeModel();
        }

        public FormDefaultComponentFieldManageModel(FormDefaultComponentField formDefaultComponentField)
            : this()
        {
            Id = formDefaultComponentField.Id;
            Name = formDefaultComponentField.Name;
            Attributes = SerializeUtilities.Deserialize<FormComponentFieldAttributeModel>(formDefaultComponentField.Attributes);

            FormDefaultComponentId = formDefaultComponentField.FormDefaultComponentId;
            FormDefaultComponents = _formDefaultComponentService.GetFormDefaultComponents(formDefaultComponentField.FormDefaultComponentId);

            RecordOrder = formDefaultComponentField.RecordOrder;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [StringLength(60)]
        [LocalizedDisplayName("FormDefaultComponentField_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("FormDefaultComponentField_Field_Attributes")]
        public FormComponentFieldAttributeModel Attributes { get; set; }

        [LocalizedDisplayName("FormDefaultComponentField_Field_FormDefaultComponentId")]
        public int FormDefaultComponentId { get; set; }

        public IEnumerable<SelectListItem> FormDefaultComponents { get; set; }

        [LocalizedDisplayName("FormDefaultComponentField_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion
    }
}
