using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Models.Locations.Widgets;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.Locations;
using System.Collections.Generic;
using System.Web;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.Locations
{
    public class LocationResolver : Widget
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
                Name = "Location",
                Widget = "Location",
                Description = "Get location and render using template",
                WidgetType = WidgetType.EzCMSContent,
                DefaultTemplate = "Default.Location",
                Type = typeof(LocationWidget),
                ManageType = typeof(WidgetManageModel)
            };
        }

        #endregion

        #region Constructors

        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly ILocationService _locationService;
        private readonly IEzCMSLocalizedResourceService _localizedResourceService;

        public LocationResolver()
        {
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            _locationService = HostContainer.GetInstance<ILocationService>();
            _localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
        }

        #endregion

        #region Parse Params
        private int LocationId { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 1, Description = "The name of template that used for rendering, this is optional parameter, if not defined then get the default Template ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Template 
             */

            LocationId = HttpContext.Current.Request.QueryString["LocationId"].ToInt(0);

            //Template 
            if (parameters.Length > 1)
            {
                Template = parameters[1];
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
                parameters.AddRange(new List<object> { Template });
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

            if (LocationId == 0)
            {
                return string.Empty;
            }

            var model = _locationService.GetLocationWidget(LocationId);

            if (model == null)
            {
                return _localizedResourceService.T("Widget_Location_Message_LocationNotFound");
            }

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return EzRazorEngineHelper.CompileAndRun(template.Content, model, null, template.CacheName);
        }
    }
}
