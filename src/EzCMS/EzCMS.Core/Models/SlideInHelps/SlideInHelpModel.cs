using Ez.Framework.Models;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.SlideInHelps
{
    public class SlideInHelpModel : BaseGridModel
    {
        public SlideInHelpModel()
        {

        }

        public SlideInHelpModel(SlideInHelp slideInHelp)
            : base(slideInHelp)
        {
            Id = slideInHelp.Id;
            Language = slideInHelp.Language.Key;
            TextKey = slideInHelp.TextKey;
            HelpTitle = slideInHelp.HelpTitle;
            MasterHelpContent = slideInHelp.MasterHelpContent;
            LocalHelpContent = slideInHelp.LocalHelpContent;
            LocalVersion = slideInHelp.LocalVersion;
            MasterVersion = slideInHelp.MasterVersion;

        }

        #region Public Properties

        public string Language { get; set; }

        public string TextKey { get; set; }

        public string HelpTitle { get; set; }

        public string LocalHelpContent { get; set; }

        public int LocalVersion { get; set; }

        public int MasterVersion { get; set; }

        public string MasterHelpContent { get; set; }

        public bool Active { get; set; }

        #endregion
    }
}
