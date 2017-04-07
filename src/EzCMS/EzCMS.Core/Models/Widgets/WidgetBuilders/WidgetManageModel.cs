using Ez.Framework.Core.Attributes;
using Ez.Framework.IoC;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Mvc.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Models.Widgets.WidgetBuilders
{
    public class WidgetManageModel
    {
        #region Constructors

        private readonly IWidgetTemplateService _widgetTemplateService;
        public WidgetManageModel()
        {
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();

            Templates = new List<SelectListItem>();
        }

        public WidgetManageModel(string[] parameters)
            : this()
        {
            ParseBaseParams(parameters);
            Templates = _widgetTemplateService.GetTemplatesOfWidget(Widget);
        }

        #endregion

        #region Public Properties

        public string Widget { get; set; }

        public string FullWidget { get; set; }

        [LocalizedDisplayName("Widget_Template")]
        public string Template { get; set; }

        public IEnumerable<SelectListItem> Templates { get; set; }

        #endregion

        #region Methods

        public void ParseBaseParams(string[] parameters)
        {
            FullWidget = string.Join(EzCMSContants.WidgetSeparator, parameters).ToWidgetFormat();
            Widget = parameters.FirstOrDefault();
        }

        #endregion
    }
}
