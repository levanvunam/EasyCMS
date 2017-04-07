using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class NotificationConfigurationSetting : ComplexSetting
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
                Name = SettingNames.NotificationConfiguration,
                SettingType = "system",
                DefaultValue = new NotificationConfigurationSetting
                {
                    DisableNotifyContacts = false
                }
            };
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SiteSetting_NotificationConfigurationSetting_Field_DisableNotifyContacts")]
        public bool DisableNotifyContacts { get; set; }

        #endregion
    }
}
