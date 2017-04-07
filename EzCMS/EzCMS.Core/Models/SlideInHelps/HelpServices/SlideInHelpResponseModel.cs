using System;

namespace EzCMS.Core.Models.SlideInHelps.HelpServices
{
    public class SlideInHelpResponseModel
    {
        public SlideInHelpResponseModel()
        {

        }

        public SlideInHelpResponseModel(SlideInHelpDictionaryItem slideInHelp)
            : this()
        {
            Language = slideInHelp.Language;
            TextKey = slideInHelp.TextKey;
            HelpTitle = slideInHelp.HelpTitle;
            HelpContent = slideInHelp.LocalHelpContent;
            Version = slideInHelp.LocalVersion;
            LastUpdate = slideInHelp.LastUpdate;
        }

        #region Public Properties

        public string TextKey { get; set; }

        public string Language { get; set; }

        public string HelpTitle { get; set; }

        public string HelpContent { get; set; }

        public int Version { get; set; }

        public DateTime LastUpdate { get; set; }

        #endregion
    }
}
