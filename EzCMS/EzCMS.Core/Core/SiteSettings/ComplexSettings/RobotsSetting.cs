using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class RobotsSetting : ComplexSetting
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
                Name = SettingNames.RobotsSetting,
                SettingType = "system",
                DefaultValue = new RobotsSetting
                {
                    LiveSiteModeContent = @"User-agent: *
Disallow:",

                    TestSiteModeContent = @"User-agent: *
Disallow: /",
                }
            };
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Content of robots.txt
        /// </summary>
        [LocalizedDisplayName("SiteSetting_RobotsSetting_Field_LiveSiteModeContent")]
        public string LiveSiteModeContent { get; set; }

        /// <summary>
        /// Content of robots.txt
        /// </summary>
        [LocalizedDisplayName("SiteSetting_RobotsSetting_Field_TestSiteModeContent")]
        public string TestSiteModeContent { get; set; }

        #endregion
    }
}