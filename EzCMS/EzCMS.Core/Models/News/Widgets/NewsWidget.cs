using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Models.NewsCategories.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EzCMS.Core.Models.News.Widgets
{
    public class NewsWidget
    {
        public NewsWidget()
        {
            Categories = new List<NewsCategoryWidget>();
        }

        public NewsWidget(Entity.Entities.Models.News news)
            : this()
        {
            Id = news.Id;
            Title = news.Title;
            Abstract = news.Abstract;
            Content = news.Content;
            ImageUrl = news.ImageUrl;
            DatePosted = news.DateStart ?? (news.LastUpdate ?? news.Created);
            LastUpdatedBy = news.LastUpdate.HasValue ? news.LastUpdateBy : news.CreatedBy;
            var newsCategories = news.NewsNewsCategories.Select(nc => nc.NewsCategory).ToList();
            Categories = newsCategories.Any()
                ? newsCategories.Select(c => new NewsCategoryWidget(c)).ToList()
                : new List<NewsCategoryWidget>();
        }

        #region Public Properties

        public int Id { get; set; }

        public string Title { get; set; }

        public string Abstract { get; set; }

        [UsingRaw]
        public string Content { get; set; }

        public string ImageUrl { get; set; }

        public DateTime DatePosted { get; set; }

        public string LastUpdatedBy { get; set; }

        public List<NewsCategoryWidget> Categories { get; set; }

        #endregion
    }
}
