using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class GoogleAnalyticApiSetting : ComplexSetting
    {
        #region Setup

        /// <summary>
        /// Get complex setting setup
        /// </summary>
        /// <returns></returns>
        public override ComplexSettingSetup GetSetup()
        {
            return new ComplexSettingSetup
            {
                Name = SettingNames.GoogleAnalyticApi,
                Description = "Google Analytic Api setting for accessing Google analytic data",
                SettingType = "system",
                DefaultValue = new GoogleAnalyticApiSetting
                {
                    ClientId = string.Empty,
                    ClientSecret = string.Empty,
                    RefreshToken = string.Empty
                }
            };
        }

        #endregion

        #region Public Properties

        [Required]
        [LocalizedDisplayName("SiteSetting_GoogleAnalyticApiSetting_Field_GoogleClientId")]
        public string ClientId { get; set; }

        [Required]
        [LocalizedDisplayName("SiteSetting_GoogleAnalyticApiSetting_Field_GoogleClientSecret")]
        public string ClientSecret { get; set; }

        [Required]
        [LocalizedDisplayName("SiteSetting_GoogleAnalyticApiSetting_Field_GoogleRefreshToken")]
        public string RefreshToken { get; set; }

        #endregion
    }
}
