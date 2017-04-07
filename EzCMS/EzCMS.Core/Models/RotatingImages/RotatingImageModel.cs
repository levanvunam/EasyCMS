using Ez.Framework.Models;

namespace EzCMS.Core.Models.RotatingImages
{
    public class RotatingImageModel : BaseGridModel
    {
        public string ImageUrl { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string UrlTarget { get; set; }

        public int GroupId { get; set; }

        public string GroupName { get; set; }
    }
}
