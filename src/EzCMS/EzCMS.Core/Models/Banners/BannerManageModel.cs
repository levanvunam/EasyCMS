using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Banners
{
    public class BannerManageModel
    {
        #region Constructors

        public BannerManageModel()
        {

        }

        public BannerManageModel(Banner banner)
            : this()
        {
            Id = banner.Id;
            ImageUrl = banner.ImageUrl;
            Text = banner.Text;
            Url = banner.Url;
            GroupName = banner.GroupName;
        }

        #endregion

        #region Public Properties
        public int? Id { get; set; }

        [LocalizedDisplayName("Banner_Field_ImageUrl")]
        public string ImageUrl { get; set; }

        [LocalizedDisplayName("Banner_Field_Text")]
        public string Text { get; set; }

        [LocalizedDisplayName("Banner_Field_Url")]
        public string Url { get; set; }

        [LocalizedDisplayName("Banner_Field_GroupName")]
        public string GroupName { get; set; }

        #endregion
    }
}
