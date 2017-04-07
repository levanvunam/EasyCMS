using System.Collections.Generic;

namespace Ez.Framework.Utilities.Web.Models
{
    public class WebPageInformationModel
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public string Keywords { get; set; }

        public List<string> ImageUrls { get; set; }

        public string ImageUrl { get; set; }
    }
}
