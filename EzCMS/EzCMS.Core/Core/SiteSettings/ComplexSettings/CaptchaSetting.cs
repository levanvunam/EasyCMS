using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class CaptchaSetting : ComplexSetting
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
                Name = SettingNames.CaptchaSetting,
                SettingType = "system",
                DefaultValue = new CaptchaSetting
                {
                    PublicKey = string.Empty,
                    PrivateKey = string.Empty
                }
            };
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SiteSetting_CaptchaSetting_Field_PublicKey")]
        public string PublicKey { get; set; }

        [LocalizedDisplayName("SiteSetting_CaptchaSetting_Field_PrivateKey")]
        public string PrivateKey { get; set; }

        #endregion
    }
}