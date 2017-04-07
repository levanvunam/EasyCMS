using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class GridRowListSetting : ComplexSetting
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
                Name = SettingNames.GridRowListSetting,
                SettingType = "system",
                DefaultValue = new GridRowListSetting
                {
                    RowList = "25,50,100,250"
                }
            };
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SiteSetting_GoogleAnalyticApiSetting_Field_RowList")]
        public string RowList { get; set; }

        #endregion
    }
}
