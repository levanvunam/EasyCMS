using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Services.WidgetManageModels;
using EzCMS.Core.Models.Services.Widgets;
using EzCMS.Core.Services.Services;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.Services
{
    public class ServicesResolver : Widget
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
                Name = "Services",
                Widget = "Services",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Get a number of services and render using template",
                DefaultTemplate = "Default.Services",
                Type = typeof(List<ServiceWidget>),
                ManageType = typeof(ServicesWidgetManageModel)
            };
        }

        #endregion

        #region Private Properties
        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly IServiceService _serviceService;
        private const int DefaultRandomNumber = 5;
        #endregion

        #region Constructors
        public ServicesResolver()
        {
            _serviceService = HostContainer.GetInstance<IServiceService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "Number", Order = 1, Description = "The number of services to get")]
        public int Number { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 2, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Count
             * * Template 
             */
            Number = DefaultRandomNumber;

            //Count
            if (parameters.Length > 1)
            {
                Number = parameters[1].ToInt(DefaultRandomNumber);
            }

            //Template 
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
                parameters.AddRange(new List<object> { Number, Template });
            }
            else if (Number > 0)
            {
                parameters.AddRange(new List<object> { Number });
            }

            return string.Join(EzCMSContants.WidgetSeparator, parameters).ToWidgetFormat();
        }

        #endregion

        /// <summary>
        /// Render Service widget
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override string Render(string[] parameters)
        {
            ParseParams(parameters);

            var model = _serviceService.GetServices(Number);

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
