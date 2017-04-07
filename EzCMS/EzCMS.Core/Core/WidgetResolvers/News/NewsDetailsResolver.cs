using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.News.WidgetManageModels;
using EzCMS.Core.Models.News.Widgets;
using EzCMS.Core.Services.News;
using System.Collections.Generic;
using System.Web;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.News
{
    public class NewsDetailsResolver : Widget
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
                Name = "News Details",
                Widget = "NewsDetails",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Get news details and render using template",
                DefaultTemplate = "Default.NewsDetails",
                Type = typeof(NewsWidget),
                ManageType = typeof(NewsWidgetManageModel)
            };
        }

        #endregion

        #region Private Properties
        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly INewsService _newsService;
        #endregion

        #region Constructors

        public NewsDetailsResolver()
        {
            _newsService = HostContainer.GetInstance<INewsService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "NewsId", Order = 1, Description = "The id of news that need to generate details")]
        public int NewsId { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 2, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * NewsId
             * * Template 
             */

            //NewsId
            if (parameters.Length > 1)
            {
                NewsId = parameters[1].ToInt(0);
            }

            if (NewsId == 0)
            {
                NewsId = HttpContext.Current.Request.QueryString["NewsId"].ToInt(0);
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
                parameters.AddRange(new List<object> { NewsId, Template });
            }
            else if (NewsId > 0)
            {
                parameters.AddRange(new List<object> { NewsId });
            }

            return string.Join(EzCMSContants.WidgetSeparator, parameters).ToWidgetFormat();
        }

        #endregion

        /// <summary>
        /// Render News widget
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override string Render(string[] parameters)
        {
            ParseParams(parameters);
            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            var model = _newsService.GetNewsDetails(NewsId);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
