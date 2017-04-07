using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class SEOScoringSetting : ComplexSetting
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
                Name = SettingNames.SEOScoringSetting,
                SettingType = "system",
                DefaultValue = new SEOScoringSetting
                {
                    TitleGoodRangeFrom = 1,
                    TitleGoodRangeTo = 55,
                    DescriptionGoodRangeFrom = 1,
                    DescriptionGoodRangeTo = 160,
                    KeywordCountGoodRangeFrom = 1,
                    KeywordCountGoodRangeTo = 5,
                    KeywordWeightGoodRangeFrom = 9,
                    KeywordWeightGoodRangeTo = 15,
                }
            };
        }

        #endregion

        #region Public Properties

        #region Title

        [LocalizedDisplayName("SiteSetting_SEOScoringSetting_Field_Title")]
        public int TitleGoodRangeFrom { get; set; }

        public int TitleGoodRangeTo { get; set; }

        #endregion

        #region Description

        [LocalizedDisplayName("SiteSetting_SEOScoringSetting_Field_Description")]
        public int DescriptionGoodRangeFrom { get; set; }

        public int DescriptionGoodRangeTo { get; set; }

        #endregion

        #region Keyword Count

        [LocalizedDisplayName("SiteSetting_SEOScoringSetting_Field_KeywordCount")]
        public int KeywordCountGoodRangeFrom { get; set; }

        public int KeywordCountGoodRangeTo { get; set; }

        #endregion

        #region Keyword Weight

        [LocalizedDisplayName("SiteSetting_SEOScoringSetting_Field_KeywordWeight")]
        public int KeywordWeightGoodRangeFrom { get; set; }

        public int KeywordWeightGoodRangeTo { get; set; }

        #endregion

        #endregion
    }
}