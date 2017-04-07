using System.Collections.Generic;
using System.Text.RegularExpressions;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Pages.Widgets.PageSearch
{
    public class PageSearchItem
    {
        public PageSearchItem()
        {

        }

        public PageSearchItem(Page page, List<string> searchItems, PageSearchSetting pageSearchSetting)
            : this()
        {
            SearchItems = searchItems;
            Title = page.Title;
            Keywords = page.Keywords ?? string.Empty;
            Description = page.Description ?? string.Empty;
            PageContent = page.Content.StripHtml();
            Abstract = page.Abstract ?? string.Empty;
            Url = page.FriendlyUrl;

            Content = PageContent.GetMatchingContent(SearchItems);


            var points = 0;
            foreach (var item in SearchItems)
            {
                points += Regex.Matches(Url, item).Count * pageSearchSetting.Url;
                points += Regex.Matches(Title, item).Count * pageSearchSetting.Title;
                points += Regex.Matches(Keywords, item).Count * pageSearchSetting.Keywords;
                points += Regex.Matches(Description, item).Count * pageSearchSetting.PageDescription;
                points += Regex.Matches(Abstract, item).Count * pageSearchSetting.Abstract;
                points += Regex.Matches(PageContent, item).Count * pageSearchSetting.Content;

                //points += Url.Split('-').Count(c => c.Contains(item, StringComparison.CurrentCultureIgnoreCase)) * pageSearchSetting.Url;
                //points += Title.Split(' ').Count(c => c.Contains(item, StringComparison.CurrentCultureIgnoreCase)) * pageSearchSetting.Title;
                //points += Keywords.Split(' ').Count(c => c.Contains(item, StringComparison.CurrentCultureIgnoreCase)) * pageSearchSetting.Keywords;
                //points += Description.Split(' ').Count(c => c.Contains(item, StringComparison.CurrentCultureIgnoreCase)) * pageSearchSetting.Description;
                //points += Abstract.Split(' ').Count(c => c.Contains(item, StringComparison.CurrentCultureIgnoreCase)) * pageSearchSetting.Abstract;
                //points += PageContent.Split(' ').Count(c => c.Contains(item, StringComparison.CurrentCultureIgnoreCase)) * pageSearchSetting.Content;
            }

            TotalPoints = points;
        }

        #region Public Properties

        public int TotalPoints { get; set; }

        public string Title { get; set; }

        public string Keywords { get; set; }

        public string Description { get; set; }

        public string Abstract { get; set; }

        public string Content { get; set; }

        public string Url { get; set; }

        #endregion

        private List<string> SearchItems { get; set; }

        private string PageContent { get; set; }
    }
}