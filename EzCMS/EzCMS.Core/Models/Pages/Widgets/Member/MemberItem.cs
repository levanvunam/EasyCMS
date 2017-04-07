using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Pages.Widgets.Member
{
    public class MemberItem
    {
        #region Contructors

        public MemberItem()
        {

        }

        public MemberItem(Page page)
            : this()
        {
            Title = page.Title;
            SeoTitle = page.SeoTitle;
            FriendlyUrl = page.FriendlyUrl;
            Keywords = page.Keywords;
            Abstract = page.Abstract;
            IsDraft = page.Status == PageEnums.PageStatus.Draft;
        }

        #endregion

        #region Public Properties

        public string Title { get; set; }

        public string SeoTitle { get; set; }

        public string FriendlyUrl { get; set; }

        public string Keywords { get; set; }

        public string Abstract { get; set; }

        public bool IsDraft { get; set; }

        public bool CanView { get; set; }

        public bool CanEdit { get; set; }

        #endregion
    }
}