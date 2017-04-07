using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Pages.Widgets
{
    public class PageLinkWidget
    {
        public PageLinkWidget()
        {

        }

        public PageLinkWidget(Page page, bool renderHtmlLink, string className, string title)
            : this()
        {
            Id = page.Id;
            Title = string.IsNullOrEmpty(title) ? page.Title : title;
            FriendlyUrl = page.FriendlyUrl;

            RenderHtmlLink = renderHtmlLink;
            ClassName = className;
        }

        #region Public Properties

        public int Id { get; set; }

        public string Title { get; set; }

        public string FriendlyUrl { get; set; }

        public bool RenderHtmlLink { get; set; }

        public string ClassName { get; set; }

        public string Label { get; set; }

        #endregion
    }
}
