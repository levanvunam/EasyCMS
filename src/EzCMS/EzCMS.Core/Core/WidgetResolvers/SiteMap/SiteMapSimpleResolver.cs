using Ez.Framework.IoC;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.ClientNavigations.Widgets;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.ClientNavigations;
using EzCMS.Core.Services.Tree;
using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.SiteMap
{
    public class SiteMapSimpleResolver : Widget
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
                Name = "SiteMap Simple",
                Widget = "SiteMapSimple",
                WidgetType = WidgetType.SiteMap,
                Description = "Render site map in simple mode without edit/create/delete action",
                DefaultTemplate = "Default.SiteMapSimple",
                Type = typeof(List<ITree<NavigationNodeModel>>),
                ManageType = typeof(WidgetManageModel)
            };
        }

        #endregion

        #region Private Properties

        private readonly IClientNavigationService _clientNavigationService;
        private readonly IWidgetTemplateService _widgetTemplateService;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public SiteMapSimpleResolver()
        {
            _clientNavigationService = HostContainer.GetInstance<IClientNavigationService>();
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

        public override string Render(string[] parameters)
        {
            ParseParams(parameters);

            var model = _clientNavigationService.GenerateSiteMap().ToList();

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
