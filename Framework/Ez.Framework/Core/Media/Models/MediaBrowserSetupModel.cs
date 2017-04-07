using Ez.Framework.Core.Enums;

namespace Ez.Framework.Core.Media.Models
{
    public class MediaBrowserSetupModel
    {
        public MediaBrowserSetupModel()
        {
            FileTreeAttribute = new FileTreeAttribute();
        }

        #region Public Properties

        public FileTreeAttribute FileTreeAttribute { get; set; }

        public string RootFolder { get; set; }

        public MediaEnums.MediaBrowserSelectMode Mode { get; set; }

        #endregion
    }
}
