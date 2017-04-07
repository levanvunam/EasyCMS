using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.FormComponentTemplates;
using EzCMS.Core.Services.FormTabs;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.FormComponents
{
    public class FormComponentSearchModel
    {
        public FormComponentSearchModel()
        {
            var formTabService = HostContainer.GetInstance<IFormTabService>();
            var formComponentTemplateService = HostContainer.GetInstance<IFormComponentTemplateService>();

            FormTabs = formTabService.GetFormTabs();
            FormComponentTemplates = formComponentTemplateService.GetFormComponentTemplates();
        }

        #region Public Properties

        public int? FormTabId { get; set; }

        public IEnumerable<SelectListItem> FormTabs { get; set; }

        public int? FormComponentTemplateId { get; set; }

        public IEnumerable<SelectListItem> FormComponentTemplates { get; set; }

        #endregion
    }
}
