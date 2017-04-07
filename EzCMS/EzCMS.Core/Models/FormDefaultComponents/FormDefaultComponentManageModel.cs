using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.FormComponentTemplates;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.FormDefaultComponents
{
    public class FormDefaultComponentManageModel
    {
        private readonly IFormComponentTemplateService _formDefaultComponentTemplateService;

        #region Constructors

        public FormDefaultComponentManageModel()
        {
            _formDefaultComponentTemplateService = HostContainer.GetInstance<IFormComponentTemplateService>();

            Templates = _formDefaultComponentTemplateService.GetFormComponentTemplates();
        }

        public FormDefaultComponentManageModel(FormDefaultComponent formDefaultComponent)
            : this()
        {
            Id = formDefaultComponent.Id;
            Name = formDefaultComponent.Name;

            FormComponentTemplateId = formDefaultComponent.FormComponentTemplateId;
            Templates = _formDefaultComponentTemplateService.GetFormComponentTemplates(formDefaultComponent.FormComponentTemplateId);

            RecordOrder = formDefaultComponent.RecordOrder;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [StringLength(60)]
        [LocalizedDisplayName("FormDefaultComponent_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("FormDefaultComponent_Field_FormDefaultComponentTemplateId")]
        public int FormComponentTemplateId { get; set; }

        public IEnumerable<SelectListItem> Templates { get; set; }
        
        [LocalizedDisplayName("FormDefaultComponent_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion
    }
}
