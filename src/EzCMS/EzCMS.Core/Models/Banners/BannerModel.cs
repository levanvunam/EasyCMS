using Ez.Framework.Models;

namespace EzCMS.Core.Models.Banners
{
    public class BannerModel : BaseGridModel
    {

        #region Public Properties
        public string ImageUrl { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public string GroupName { get; set; }

        #endregion
    }
}
