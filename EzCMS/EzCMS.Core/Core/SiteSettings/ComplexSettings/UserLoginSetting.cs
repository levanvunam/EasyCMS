using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class UserLoginSetting : ComplexSetting
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
                Name = SettingNames.UserLoginSettings,
                SettingType = "system",
                DefaultValue = new UserLoginSetting
                {
                    LockOutTimeInMinutes = 10,
                    MaxAllowedLoginAttempts = 5,
                    ResetPasswordEffectiveTimeInMinutes = 30,
                    PasswordExpiryDays = 0
                }
            };
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SiteSetting_UserLoginSetting_Field_LockOutTimeInMinutes")]
        public int LockOutTimeInMinutes { get; set; }

        [LocalizedDisplayName("SiteSetting_UserLoginSetting_Field_MaxAllowedLoginAttempts")]
        public int MaxAllowedLoginAttempts { get; set; }

        [LocalizedDisplayName("SiteSetting_UserLoginSetting_Field_ResetPasswordEffectiveTimeInMinutes")]
        public int ResetPasswordEffectiveTimeInMinutes { get; set; }

        [LocalizedDisplayName("SiteSetting_UserLoginSetting_Field_PasswordExpiryDays")]
        public int PasswordExpiryDays { get; set; }

        #endregion
    }
}