using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.RssFeeds.WidgetManageModels;
using EzCMS.Core.Models.RssFeeds.Widgets;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.RssFeeds;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.RssFeeds
{
    public class RssFeedResolver : Widget
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
                Name = "RssFeed",
                Widget = "RssFeed",
                Description = "Get rss feed and render using template",
                WidgetType = WidgetType.EzCMSContent,
                DefaultTemplate = "Default.RssFeed",
                Type = typeof(RssFeedWidget),
                ManageType = typeof(RssFeedWidgetManageModel)
            };
        }

        #endregion

        #region Constructors

        private readonly IRssFeedService _rssFeedService;
        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly IEzCMSLocalizedResourceService _localizedResourceService;

        public RssFeedResolver()
        {
            _rssFeedService = HostContainer.GetInstance<IRssFeedService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            _localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "RssFeedId", Order = 1, Description = "The id of rss feed that need to to get")]
        public int RssFeedId { get; set; }

        [WidgetParam(Name = "Number", Order = 2, Description = "Display number of items")]
        public int? Number { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 3, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * RssFeedId
             * * Number
             * * Template 
             */

            //RssFeedId
            if (parameters.Length > 1)
            {
                RssFeedId = parameters[1].ToInt();
            }

            //Number
            if (parameters.Length > 2)
            {
                Number = parameters[2].ToInt();
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
                parameters.AddRange(new List<object> { RssFeedId, Number, Template });
            }
            else if (Number.HasValue)
            {
                parameters.AddRange(new List<object> { Number, Template });
            }
            else if (RssFeedId > 0)
            {
                parameters.AddRange(new List<object> { RssFeedId });
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

            var model = _rssFeedService.GetRssFeedWidget(RssFeedId, Number);

            if (model == null)
            {
                return _localizedResourceService.T("Widget_RSSFeeds_Message_InvalidRssFeedId");
            }

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
