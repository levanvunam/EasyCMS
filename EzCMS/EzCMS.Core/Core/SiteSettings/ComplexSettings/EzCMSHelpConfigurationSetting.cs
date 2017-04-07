using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.SiteSettings;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class EzCMSHelpConfigurationSetting : ComplexSetting
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
                Name = SettingNames.EzCMSHelpConfiguration,
                SettingType = "system",
                DefaultValue = new EzCMSHelpConfigurationSetting
                {
                    DisabledSlideInHelpService = false,
                    DisabledOnlineBodyTemplateService = false,
                    HelpServiceUrl = EzCMSContants.EzCMSServiceUrl,
                    AuthorizeCode = string.Empty
                }
            };
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SiteSetting_EzCMSHelpConfigurationSetting_Field_HelpServiceUrl")]
        public string HelpServiceUrl { get; set; }

        [LocalizedDisplayName("SiteSetting_EzCMSHelpConfigurationSetting_Field_DisableHelpService")]
        public bool DisabledSlideInHelpService { get; set; }

        [LocalizedDisplayName("SiteSetting_EzCMSHelpConfigurationSetting_Field_DisabledOnlineBodyTemplateService")]
        public bool DisabledOnlineBodyTemplateService { get; set; }

        [LocalizedDisplayName("SiteSetting_EzCMSHelpConfigurationSetting_Field_AuthorizeCode")]
        public string AuthorizeCode { get; set; }

        #endregion
    }
}
