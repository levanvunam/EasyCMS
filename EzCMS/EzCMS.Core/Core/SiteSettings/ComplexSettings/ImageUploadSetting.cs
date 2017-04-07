using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class ImageUploadSetting : ComplexSetting
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
                Name = SettingNames.ImageUploadSetting,
                SettingType = "system",
                DefaultValue = new ImageUploadSetting
                {
                    MinWidth = null,
                    MinHeight = null,
                    MaxWidth = null,
                    MaxHeight = null
                }
            };
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SiteSetting_ImageUploadSetting_Field_MinWidth")]
        public int? MinWidth { get; set; }

        [LocalizedDisplayName("SiteSetting_ImageUploadSetting_Field_MinHeight")]
        public int? MinHeight { get; set; }

        [LocalizedDisplayName("SiteSetting_ImageUploadSetting_Field_MaxWidth")]
        public int? MaxWidth { get; set; }

        [LocalizedDisplayName("SiteSetting_ImageUploadSetting_Field_MaxHeight")]
        public int? MaxHeight { get; set; }

        #endregion
    }
}
