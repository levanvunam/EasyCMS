using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class PasswordSetting : ComplexSetting
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
                Name = SettingNames.PasswordSetting,
                SettingType = "system",
                DefaultValue = new PasswordSetting
                {
                    PasswordMinLengthRequired = 8,
                    PasswordMustHaveDigit = false,
                    PasswordMustHaveSymbol = false,
                    PasswordMustHaveUpperAndLowerCaseLetters = false
                }
            };
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SiteSetting_PasswordSetting_Field_PasswordMinLengthRequired")]
        public int PasswordMinLengthRequired { get; set; }

        [LocalizedDisplayName("SiteSetting_PasswordSetting_Field_PasswordMustHaveUpperAndLowerCaseLetters")]
        public bool PasswordMustHaveUpperAndLowerCaseLetters { get; set; }

        [LocalizedDisplayName("SiteSetting_PasswordSetting_Field_PasswordMustHaveDigit")]
        public bool PasswordMustHaveDigit { get; set; }

        [LocalizedDisplayName("SiteSetting_PasswordSetting_Field_PasswordMustHaveSymbol")]
        public bool PasswordMustHaveSymbol { get; set; }

        #endregion
    }
}
