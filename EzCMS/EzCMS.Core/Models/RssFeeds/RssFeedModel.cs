using Ez.Framework.Models;
using EzCMS.Entity.Core.Enums;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.RssFeeds
{
    public class RssFeedModel : BaseGridModel
    {
        #region Public Properties

        public string Name { get; set; }

        public RssFeedEnums.RssType RssType { get; set; }

        public string RssTypeName
        {
            get
            {
                return RssType.GetEnumName();
            }
        }

        public string Url { get; set; }

        #endregion
    }
}
