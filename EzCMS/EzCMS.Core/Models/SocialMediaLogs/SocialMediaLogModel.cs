using Ez.Framework.Models;

namespace EzCMS.Core.Models.SocialMediaLogs
{
    public class SocialMediaLogModel : BaseGridModel
    {
        #region Public Properties

        public int SocialMediaId { get; set; }

        public string SocialMedia { get; set; }

        public int SocialMediaTokenId { get; set; }

        public string SocialMediaToken { get; set; }

        public int PageId { get; set; }

        public string PageTitle { get; set; }

        public string PostedContent { get; set; }

        public string PostedResponse { get; set; }

        #endregion
    }
}
