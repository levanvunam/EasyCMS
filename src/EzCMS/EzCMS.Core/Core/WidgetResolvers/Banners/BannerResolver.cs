using Ez.Framework.Core.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Models.Banners.WidgetManageModels;
using EzCMS.Core.Models.Banners.Widgets;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Services.Banners;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.WidgetTemplates;
using System.Collections.Generic;

namespace EzCMS.Core.Core.WidgetResolvers.Banners
{
    public class BannerResolver : Widget
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
                Name = "Banner",
                Widget = "Banner",
                Description = "Get single banner and render using template",
                WidgetType = WidgetType.EzCMSContent,
                DefaultTemplate = "Default.Banner",
                Type = typeof(BannerWidget),
                ManageType = typeof(BannerWidgetManageModel)
            };
        }

        #endregion

        #region Private Properties

        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly IBannerService _bannerService;
        private readonly IEzCMSLocalizedResourceService _localizedResourceService;

        #endregion

        #region Constructors
        public BannerResolver()
        {
            _bannerService = HostContainer.GetInstance<IBannerService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            _localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "BannerId", Order = 1, Description = "The id of banner that need to to get")]
        public int BannerId { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 2, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * BannerId
             * * Template 
             */

            //BannerId
            if (parameters.Length > 1)
            {
                BannerId = parameters[1].ToInt();
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
                GetSetup().Widget, BannerId
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

            var model = _bannerService.GetBannerWidget(BannerId);

            if (model == null)
            {
                return _localizedResourceService.T("Widget_Banner_Message_InvalidBannerId");
            }

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}