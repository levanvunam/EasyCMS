using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Banners.Widgets
{
    public class BannerWidget
    {
        public BannerWidget()
        {

        }

        public BannerWidget(Banner banner)
        {
            Id = banner.Id;
            ImageUrl = banner.ImageUrl;
            Text = banner.Text;
            Url = banner.Url;
            GroupName = banner.GroupName;
        }

        #region Public Properties

        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public string GroupName { get; set; }

        #endregion
    }
}
