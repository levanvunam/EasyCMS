using Ez.Framework.IoC;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Models.Subscriptions.Widgets.ContentUpdates;
using EzCMS.Core.Services.Subscriptions;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.Subscriptions
{
    public class ContentUpdateResolver : Widget
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
                Name = "Content updates information",
                Widget = "ContentUpdates",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Render all content updates from last login",
                DefaultTemplate = "Default.ContentUpdates",
                Type = typeof(ContentUpdateWidget),
                ManageType = typeof(WidgetManageModel)
            };
        }

        #endregion

        #region Private Properties

        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly ISubscriptionService _subscriptionService;

        #endregion

        #region Constructors
        public ContentUpdateResolver()
        {
            _subscriptionService = HostContainer.GetInstance<ISubscriptionService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "TemplateName", Order = 1, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Template 
             */

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
                parameters.Add(Template);
            }

            return string.Join(EzCMSContants.WidgetSeparator, parameters).ToWidgetFormat();
        }

        #endregion

        /// <summary>
        /// Render testimonial widget
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override string Render(string[] parameters)
        {
            ParseParams(parameters);

            var model = _subscriptionService.GetContentUpdates();

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                                                      _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
