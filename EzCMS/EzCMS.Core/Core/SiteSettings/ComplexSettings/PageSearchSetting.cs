using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class PageSearchSetting : ComplexSetting
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
                Name = SettingNames.PageSearchSetting,
                SettingType = "system",
                DefaultValue = new PageSearchSetting
                {
                    Url = 6,
                    Title = 5,
                    Keywords = 4,
                    PageDescription = 3,
                    Abstract = 2,
                    Content = 1
                }
            };
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SiteSetting_PageSearch_Field_Url")]
        public int Url { get; set; }

        [LocalizedDisplayName("SiteSetting_PageSearch_Field_Title")]
        public int Title { get; set; }

        [LocalizedDisplayName("SiteSetting_PageSearch_Field_Keywords")]
        public int Keywords { get; set; }

        [LocalizedDisplayName("SiteSetting_PageSearch_Field_PageDescription")]
        public int PageDescription { get; set; }

        [LocalizedDisplayName("SiteSetting_PageSearch_Field_Abstract")]
        public int Abstract { get; set; }

        [LocalizedDisplayName("SiteSetting_PageSearch_Field_Content")]
        public int Content { get; set; }

        #endregion
    }
}
