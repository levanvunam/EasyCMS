using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Pages.WidgetManageModels;
using EzCMS.Core.Models.Pages.Widgets;
using EzCMS.Core.Services.Widgets;
using EzCMS.Core.Services.Pages;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.Pages
{
    public class PageLinkResolver : Widget
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
                Name = "Page Link",
                Widget = "PageLink",
                WidgetType = WidgetType.Pages,
                Description = "Generate link for page and render using template",
                DefaultTemplate = "Default.PageLink",
                Type = typeof(PageLinkWidget),
                ManageType = typeof(PageLinkWidgetManageModel),
                IsFavourite = true
            };
        }

        #endregion

        #region Private Properties
        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly IPageService _pageService;
        private readonly IWidgetService _widgetService;

        #endregion

        #region Constructors

        public PageLinkResolver()
        {
            _pageService = HostContainer.GetInstance<IPageService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            _widgetService = HostContainer.GetInstance<IWidgetService>();

        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "PageId", Order = 1, Description = "The id of page that need to generate link")]
        public int PageId { get; set; }

        [WidgetParam(Name = "RenderHtmlLink", Order = 2, Description = "Render html link or not")]
        public bool RenderHtmlLink { get; set; }

        [WidgetParam(Name = "ClassName", Order = 3, Description = "The class of link")]
        public string ClassName { get; set; }

        [WidgetParam(Name = "Label", Order = 4, Description = "The label of link")]
        public string Title { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 5, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Page Id
             * * RenderHtmlLink
             * * ClassName
             * * Label
             * * Template 
             */

            //Page Id
            if (parameters.Length > 1)
            {
                PageId = parameters[1].ToInt();
            }

            //RenderHtmlLink
            if (parameters.Length > 2)
            {
                RenderHtmlLink = parameters[2].ToBool();
            }

            //ClassName
            if (parameters.Length > 3)
            {
                ClassName = parameters[3];
            }

            //Label
            if (parameters.Length > 4)
            {
                Title = parameters[4];
            }

            //Template 
            if (parameters.Length > 5)
            {
                Template = parameters[5];
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
                parameters.AddRange(new List<object> { PageId, RenderHtmlLink, ClassName, Title, Template });
            }
            else if (!string.IsNullOrEmpty(Title))
            {
                parameters.AddRange(new List<object> { PageId, RenderHtmlLink, ClassName, Title });
            }
            else if (!string.IsNullOrEmpty(ClassName))
            {
                parameters.AddRange(new List<object> { PageId, RenderHtmlLink, ClassName });
            }
            else if (RenderHtmlLink)
            {
                parameters.AddRange(new List<object> { PageId, RenderHtmlLink });
            }
            else if (PageId > 0)
            {
                parameters.AddRange(new List<object> { PageId });
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

            var model = _pageService.GetPageLinkWidget(PageId, RenderHtmlLink, ClassName, Title);

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
