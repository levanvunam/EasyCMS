using System;
using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Services.Pages;
using Ez.Framework.IoC;
using EzCMS.Core.Services.SiteSettings;

namespace EzCMS.Core.Models.Pages.Widgets.PageSearch
{
    public class PageSearchWidget
    {
        private readonly IPageService _pageService;
        public PageSearchWidget()
        {
            _pageService = HostContainer.GetInstance<IPageService>();
            SearchResults = new List<PageSearchItem>();
            SearchItems = new List<string>();
        }

        public PageSearchWidget(string keyword)
            : this()
        {
            // Remove multiple space
            do
            {
                keyword = keyword.Replace("  ", " ");
            } while (keyword.Contains("  "));
            Keyword = keyword;

            var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
            var pageSearchSetting = siteSettingService.LoadSetting<PageSearchSetting>();

            // Split keyword for searching and rating
            SearchItems = keyword.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var pages = _pageService.GetAccessablePages().ToList();
            var searchResults = pages.Select(p => new PageSearchItem(p, SearchItems, pageSearchSetting)).ToList();
            SearchResults = searchResults.Where(s => s.TotalPoints > 0).OrderByDescending(s => s.TotalPoints).ToList();

            Total = SearchResults.Count;
        }

        #region Public Properties

        public int Total { get; set; }

        public string Keyword { get; set; }

        public List<string> SearchItems { get; set; }

        public List<PageSearchItem> SearchResults { get; set; }

        #endregion
    }
}
