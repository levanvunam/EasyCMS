using Ez.Framework.IoC;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Models.Pages.Widgets.PageSearch;
using EzCMS.Core.Services.Pages;
using System.Collections.Generic;
using System.Web;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.Pages
{
    public class PageSearchResolver : Widget
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
                Name = "Page Search",
                Widget = "PageSearch",
                WidgetType = WidgetType.Pages,
                Description = "Load search result for searching pages by keyword",
                DefaultTemplate = "Default.PageSearch",
                Type = typeof(PageSearchWidget),
                ManageType = typeof(WidgetManageModel)
            };
        }

        #endregion

        #region Private Properties
        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly IPageService _pageService;

        #endregion

        #region Constructors
        public PageSearchResolver()
        {
            _pageService = HostContainer.GetInstance<IPageService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();

        }

        #endregion

        #region Parse Params

        private string Keyword { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 1, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Template 
             */

            //Keyword
            Keyword = HttpContext.Current.Request["keyword"];

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
        /// Render widget
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override string Render(string[] parameters)
        {
            if (HttpContext.Current.Items["RenderSearchAlready"] == null)
            {
                // Prevent duplicate render
                HttpContext.Current.Items["RenderSearchAlready"] = new object();

                ParseParams(parameters);

                var model = _pageService.SearchPages(Keyword);

                var template = _widgetTemplateService.GetTemplateByName(Template) ?? _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

                return _widgetTemplateService.ParseTemplate(template, model);
            }

            return string.Empty;
        }
    }
}
