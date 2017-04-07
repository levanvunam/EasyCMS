using System;
using System.IO;
using System.Net;
using System.Text;
using Ez.Framework.Core.Mvc.Helpers;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.Common;
using EzCMS.Core.Services.SiteSettings;
using Newtonsoft.Json.Linq;

namespace EzCMS.Core.Services.Common
{
    public class CommonService : ServiceHelper, ICommonService
    {
        private readonly ISiteSettingService _siteSettingService;

        public CommonService(ISiteSettingService siteSettingService)
        {
            _siteSettingService = siteSettingService;
        }

        /// <summary>
        /// Get google analytic model
        /// </summary>
        /// <returns></returns>
        public GoogleAnalyticDashboardModel GetGoogleAnalyticAccessToken()
        {
            if (!string.IsNullOrEmpty(WorkContext.GoogleAnalyticAccessToken))
            {
                return new GoogleAnalyticDashboardModel
                {
                    AccessToken = WorkContext.GoogleAnalyticAccessToken,
                    IsSetup = true,
                    IsSetupInformationValid = true
                };
            }


            const string googleApiUrl = "https://www.googleapis.com/oauth2/v3/token";
            var googleAnalyticSetting = _siteSettingService.LoadSetting<GoogleAnalyticApiSetting>();


            if (!string.IsNullOrEmpty(googleAnalyticSetting.ClientId) &&
                !string.IsNullOrEmpty(googleAnalyticSetting.ClientSecret) &&
                !string.IsNullOrEmpty(googleAnalyticSetting.RefreshToken))
            {
                try
                {
                    var url = string.Format(
                        "client_secret={0}&grant_type=refresh_token&refresh_token={1}&client_id={2}",
                        googleAnalyticSetting.ClientSecret,
                        googleAnalyticSetting.RefreshToken,
                        googleAnalyticSetting.ClientId);

                    var req = WebRequest.Create(googleApiUrl);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";

                    var encoder = new ASCIIEncoding();

                    var data = encoder.GetBytes(url);
                    req.ContentLength = data.Length;
                    req.GetRequestStream().Write(data, 0, data.Length);

                    var resp = (HttpWebResponse) req.GetResponse();
                    var status = resp.StatusDescription;

                    if (status == "OK")
                    {
                        var responseStream = resp.GetResponseStream();
                        if (responseStream != null)
                        {
                            var tokenResponse = new StreamReader(responseStream).ReadToEnd();
                            var token = JToken.Parse(tokenResponse);
                            WorkContext.GoogleAnalyticAccessToken = token["access_token"].ToString();

                            return new GoogleAnalyticDashboardModel
                            {
                                AccessToken = WorkContext.GoogleAnalyticAccessToken,
                                IsSetup = true,
                                IsSetupInformationValid = true
                            };
                        }
                    }
                }
                catch (Exception exception)
                {
                    return new GoogleAnalyticDashboardModel
                    {
                        IsSetup = true,
                        IsSetupInformationValid = false,
                        Message = exception.Message
                    };
                }
            }

            return new GoogleAnalyticDashboardModel
            {
                IsSetup = false,
                IsSetupInformationValid = false
            };
        }
    }
}