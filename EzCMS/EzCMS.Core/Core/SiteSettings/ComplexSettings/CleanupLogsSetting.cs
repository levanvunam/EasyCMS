using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class CleanupLogsSetting : ComplexSetting
    {
        private const int DefaultConfigDays = 180;

        #region Setup

        /// <summary>
        /// Get complex setting setup
        /// </summary>
        /// <returns></returns>
        public override ComplexSettingSetup GetSetup()
        {
            return new ComplexSettingSetup
            {
                Name = SettingNames.CleanupLogsConfiguration,
                SettingType = "system",
                DefaultValue = new CleanupLogsSetting
                {
                    PageMaxBackupLogDays = DefaultConfigDays,
                    PageTemplateMaxBackupLogDays = DefaultConfigDays,
                    ProtectedDocumentMaxBackupLogDays = DefaultConfigDays,
                    ScriptMaxBackupLogDays = DefaultConfigDays,
                    SocialMediaMaxBackupLogDays = DefaultConfigDays,
                    StyleMaxBackupLogDays = DefaultConfigDays,
                    SubscriptionMaxBackupLogDays = DefaultConfigDays,
                    TemplateMaxBackupLogDays = DefaultConfigDays
                }
            };
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SiteSetting_CleanupLogsSetting_Field_PageMaxBackupLogDays")]
        public int PageMaxBackupLogDays { get; set; }

        [LocalizedDisplayName("SiteSetting_CleanupLogsSetting_Field_PageTemplateMaxBackupLogDays")]
        public int PageTemplateMaxBackupLogDays { get; set; }

        [LocalizedDisplayName("SiteSetting_CleanupLogsSetting_Field_ProtectedDocumentMaxBackupLogDays")]
        public int ProtectedDocumentMaxBackupLogDays { get; set; }

        [LocalizedDisplayName("SiteSetting_CleanupLogsSetting_Field_ScriptMaxBackupLogDays")]
        public int ScriptMaxBackupLogDays { get; set; }

        [LocalizedDisplayName("SiteSetting_CleanupLogsSetting_Field_SocialMediaMaxBackupLogDays")]
        public int SocialMediaMaxBackupLogDays { get; set; }

        [LocalizedDisplayName("SiteSetting_CleanupLogsSetting_Field_StyleMaxBackupLogDays")]
        public int StyleMaxBackupLogDays { get; set; }

        [LocalizedDisplayName("SiteSetting_CleanupLogsSetting_Field_SubscriptionMaxBackupLogDays")]
        public int SubscriptionMaxBackupLogDays { get; set; }

        [LocalizedDisplayName("SiteSetting_CleanupLogsSetting_Field_TemplateMaxBackupLogDays")]
        public int TemplateMaxBackupLogDays { get; set; }

        #endregion
    }
}
