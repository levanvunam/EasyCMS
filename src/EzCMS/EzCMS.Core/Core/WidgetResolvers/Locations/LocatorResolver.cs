using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Mvc.Helpers;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Locations.WidgetManageModels;
using EzCMS.Core.Models.Locations.Widgets;
using System.Collections.Generic;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.Locations
{
    public class LocatorResolver : Widget
    {
        #region Setup

        /// <summary>
        /// Get widget information
        /// </summary>
        /// <returns></returns>
        public override WidgetSetupModel GetSetup()
        {
            return new WidgetSetupModel
            {
                Name = "Locator",
                Widget = "Locator",
                Description = "Load locator and render using template",
                WidgetType = WidgetType.EzCMSContent,
                DefaultTemplate = "Default.Locator",
                Type = typeof(LocatorWidget),
                ManageType = typeof(LocatorWidgetManageModel)
            };
        }

        #endregion

        #region Constructors

        private readonly IWidgetTemplateService _widgetTemplateService;

        public LocatorResolver()
        {
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "Country", Order = 1, Description = "Default country to search location by postcode")]
        public string Country { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 2, Description = "The name of template that used for rendering, this is optional parameter, if not defined then get the default Template ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
            * Params:
            * * Country
            * * Template 
            */

            // Country
            if (parameters.Length > 1)
            {
                Country = parameters[1];
            }

            // Template 
            if (parameters.Length > 2)
            {
                Template = parameters[2];
            }
        }

        #endregion

        #region Generate Widget

        /// <summary>
        /// Generate full widget from params
        /// </summary>
        /// <returns></returns>
        public override string GenerateFullWidget()
        {
            var parameters = new List<object>
            {
                GetSetup().Widget
            };

            if (!string.IsNullOrEmpty(Template))
            {
                parameters.AddRange(new List<object> { Country, Template });
            }
            else if (!string.IsNullOrEmpty(Country))
            {
                parameters.AddRange(new List<object> { Country });
            }

            return string.Join(EzCMSContants.WidgetSeparator, parameters).ToWidgetFormat();
        }

        #endregion

        /// <summary>
        /// Render widget
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override string Render(string[] parameters)
        {
            ParseParams(parameters);

            var model = new LocatorWidget(Country);

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return EzRazorEngineHelper.CompileAndRun(template.Content, model, null, template.CacheName);
        }
    }
}
