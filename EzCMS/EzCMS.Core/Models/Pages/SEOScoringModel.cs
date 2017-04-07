using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Core.Models.Pages
{
    public class SEOScoringModel
    {
        #region Constructors

        public SEOScoringModel()
        {
            Title = PageEnums.SEOScore.Bad;
            Description = PageEnums.SEOScore.Bad;
            KeywordCount = PageEnums.SEOScore.Bad;
            KeywordWeight = PageEnums.SEOScore.Bad;
            KeywordBolded = PageEnums.SEOScore.Bad;
            HeadingTag = PageEnums.SEOScore.Bad;
            AltTag = PageEnums.SEOScore.Bad;
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("Page_SEOScoring_Field_Title")]
        public PageEnums.SEOScore Title { get; set; }

        [LocalizedDisplayName("Page_SEOScoring_Field_Description")]
        public PageEnums.SEOScore Description { get; set; }

        [LocalizedDisplayName("Page_SEOScoring_Field_KeywordCount")]
        public PageEnums.SEOScore KeywordCount { get; set; }

        [LocalizedDisplayName("Page_SEOScoring_Field_KeywordWeight")]
        public PageEnums.SEOScore KeywordWeight { get; set; }

        [LocalizedDisplayName("Page_SEOScoring_Field_KeywordBolded")]
        public PageEnums.SEOScore KeywordBolded { get; set; }

        [LocalizedDisplayName("Page_SEOScoring_Field_HeadingTag")]
        public PageEnums.SEOScore HeadingTag { get; set; }

        [LocalizedDisplayName("Page_SEOScoring_Field_AltTag")]
        public PageEnums.SEOScore AltTag { get; set; }

        #endregion
    }
}
