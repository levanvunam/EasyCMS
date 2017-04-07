using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.FormComponentTemplates;
using EzCMS.Core.Services.FormTabs;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.FormComponents
{
    public class FormComponentManageModel
    {
        private readonly IFormTabService _formTabService;
        private readonly IFormComponentTemplateService _formComponentTemplateService;

        #region Constructors

        public FormComponentManageModel()
        {
            _formTabService = HostContainer.GetInstance<IFormTabService>();
            _formComponentTemplateService = HostContainer.GetInstance<IFormComponentTemplateService>();

            FormTabs = _formTabService.GetFormTabs();
            Templates = _formComponentTemplateService.GetFormComponentTemplates();
        }

        public FormComponentManageModel(FormComponent formComponent)
            : this()
        {
            Id = formComponent.Id;
            Name = formComponent.Name;

            FormTabId = formComponent.FormTabId;
            FormTabs = _formTabService.GetFormTabs(formComponent.FormTabId);

            FormComponentTemplateId = formComponent.FormComponentTemplateId;
            Templates = _formComponentTemplateService.GetFormComponentTemplates(formComponent.FormComponentTemplateId);

            RecordOrder = formComponent.RecordOrder;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [StringLength(60)]
        [LocalizedDisplayName("FormComponent_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("FormComponent_Field_FormTabId")]
        public int FormTabId { get; set; }

        public IEnumerable<SelectListItem> FormTabs { get; set; }

        [LocalizedDisplayName("FormComponent_Field_FormComponentTemplateId")]
        public int FormComponentTemplateId { get; set; }

        public IEnumerable<SelectListItem> Templates { get; set; }
        
        [LocalizedDisplayName("FormComponent_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion
    }
}
