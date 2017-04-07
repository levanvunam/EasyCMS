using Ez.Framework.Core.IoC;
using EzCMS.Core.Models.News.Widgets;
using EzCMS.Core.Services.News;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.NewsCategories.Widgets
{
    public class NewsCategoryWidget
    {
        private readonly INewsService _newsServices;
        public NewsCategoryWidget()
        {
            _newsServices = HostContainer.GetInstance<INewsService>();
            NewsListing = new NewsListingWidget();
        }

        public NewsCategoryWidget(NewsCategory category, bool getNews = false)
            : this()
        {
            Id = category.Id;
            Name = category.Name;
            Abstract = category.Abstract;
            ParentId = category.ParentId;
            ParentName = category.ParentId.HasValue ? category.Parent.Name : string.Empty;
            Total = category.NewsNewsCategories.Count;
            /*
             * Prevent endless loop between NewsListingWidget and NewsCategoryWidget
             */
            if (getNews)
            {
                NewsListing = _newsServices.GetNewsListing(0, 0, 0, 0, category.Id);
            }
        }

        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public string Abstract { get; set; }

        public int? ParentId { get; set; }

        public string ParentName { get; set; }

        public int Total { get; set; }

        public NewsListingWidget NewsListing { get; set; }

        #endregion
    }
}