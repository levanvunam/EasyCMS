using Ez.Framework.IoC;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.SiteSettings;

namespace EzCMS.Core.Core.WidgetResolvers.Common
{
    public class GoogleAnalyticResolver : Widget
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
                Name = "Google Tracking Code",
                Widget = "GoogleAnalytic",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Get google analytic tracking code",
                IsFavourite = true
            };
        }

        #endregion

        #region Private Properties
        private readonly ISiteSettingService _siteSettingService;

        #endregion

        #region Constructors
        public GoogleAnalyticResolver()
        {
            _siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
        }

        #endregion

        #region Parse Params

        //No Params

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
            var script = string.Empty;
            if (WorkContext.EnableLiveSiteMode)
            {
                script = _siteSettingService.GetSetting<string>(SettingNames.GoogleAnalyticConfiguration);
            }

            return script;
        }
    }
}
