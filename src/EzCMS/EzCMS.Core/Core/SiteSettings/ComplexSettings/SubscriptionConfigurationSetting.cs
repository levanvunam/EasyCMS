using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class SubscriptionConfigurationSetting : ComplexSetting
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
                Name = SettingNames.SubscriptionConfiguration,
                SettingType = "system",
                DefaultValue = new SubscriptionConfigurationSetting
                {
                    DisableNotifySubscribers = false
                }
            };
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SiteSetting_SubscriptionConfigurationSetting_Field_DisableNotifySubscribers")]
        public bool DisableNotifySubscribers { get; set; }

        #endregion
    }
}
