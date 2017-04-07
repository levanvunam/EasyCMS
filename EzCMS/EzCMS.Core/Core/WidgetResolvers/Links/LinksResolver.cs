using Ez.Framework.Core.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Models.Links.WidgetManageModels;
using EzCMS.Core.Models.Links.Widgets;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Services.Links;
using EzCMS.Core.Services.WidgetTemplates;
using System.Collections.Generic;

namespace EzCMS.Core.Core.WidgetResolvers.Links
{
    public class LinksResolver : Widget
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
                Name = "Links Display",
                Widget = "DisplayLinks",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Get links and render using template",
                DefaultTemplate = "Default.DisplayLinks",
                Type = typeof(LinksWidget),
                ManageType = typeof(LinksWidgetManageModel)
            };
        }

        #endregion

        #region Private Properties
        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly ILinkService _linkService;

        #endregion

        #region Constructors
        public LinksResolver()
        {
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            _linkService = HostContainer.GetInstance<ILinkService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "LinkTypeId", Order = 1, Description = "The Id of link type")]
        public int LinkTypeId { get; set; }

        [WidgetParam(Name = "Number", Order = 2, Description = "The number of links to display")]
        public int Number { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 3, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * LinkTypeId
             * * Number
             * * Template 
             */

            //LinkTypeId
            if (parameters.Length > 1)
            {
                LinkTypeId = parameters[1].ToInt(0);
            }

            //Number
            if (parameters.Length > 2)
            {
                Number = parameters[2].ToInt(0);
            }

            //Template 
            if (parameters.Length > 3)
            {
                Template = parameters[3];
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
                parameters.AddRange(new List<object> { LinkTypeId, Number, Template });
            }
            else if (Number > 0)
            {
                parameters.AddRange(new List<object> { LinkTypeId, Number });
            }
            else if (LinkTypeId > 0)
            {
                parameters.AddRange(new List<object> { LinkTypeId });
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

            var model = _linkService.GetLinksWidget(LinkTypeId, Number);

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
