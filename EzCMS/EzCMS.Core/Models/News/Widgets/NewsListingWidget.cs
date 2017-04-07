using System.Collections.Generic;
using EzCMS.Core.Models.Shared;

namespace EzCMS.Core.Models.News.Widgets
{
    public class NewsListingWidget : PagingModel
    {
        public NewsListingWidget()
        {
            NewsListing = new List<NewsWidget>();
        }

        #region Public Properties

        public List<NewsWidget> NewsListing { get; set; } 

        #endregion
    }
}