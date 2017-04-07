using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.SocialMedia.Widget;
using EzCMS.Core.Models.SocialMedia.WidgetManageModels;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.SocialMediaTokens;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.SocialMedia
{
    public class SocialFeedResolver : Widget
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
                Name = "Social Feed",
                Widget = "SocialFeed",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Get social feed and render using template",
                DefaultTemplate = "Default.SocialFeed",
                Type = typeof(SocialFeedWidget),
                ManageType = typeof(SocialFeedWidgetManageModel),
                IsFavourite = true
            };
        }

        #endregion

        #region Private Properties

        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly ISocialMediaTokenService _socialMediaTokenService;
        private readonly IEzCMSLocalizedResourceService _localizedResourceService;

        #endregion

        #region Constructors

        public SocialFeedResolver()
        {
            _socialMediaTokenService = HostContainer.GetInstance<ISocialMediaTokenService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            _localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "TokenId", Order = 1, Description = "The token id of social to load")]
        public int TokenId { get; set; }

        [WidgetParam(Name = "Total", Order = 2, Description = "The total items to get")]
        public int Total { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 3, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * TokenId
             * * Total
             * * Template 
             */

            //TokenId
            if (parameters.Length > 1)
            {
                TokenId = parameters[1].ToInt(0);
            }

            //Total
            if (parameters.Length > 2)
            {
                Total = parameters[2].ToInt(0);
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
                parameters.AddRange(new List<object> { TokenId, Total, Template });
            }
            else if (Total > 0)
            {
                parameters.AddRange(new List<object> { TokenId, Total });
            }
            else if (TokenId > 0)
            {
                parameters.AddRange(new List<object> { TokenId });
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

            var model = _socialMediaTokenService.GetFeed(TokenId, Total);

            if (model == null)
            {
                return _localizedResourceService.T("Widget_SocialFeed_Message_InvalidToken");
            }

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
