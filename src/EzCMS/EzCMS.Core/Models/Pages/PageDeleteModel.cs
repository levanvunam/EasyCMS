using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.Pages;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Pages
{
    public class PageDeleteModel
    {
        private readonly IPageService _pageService;
        public PageDeleteModel()
        {
            _pageService = HostContainer.GetInstance<IPageService>();

            ReferencePages = new List<PageDeleteItemModel>();
            UrlTypes = EnumUtilities.GetAllItems<CommonEnums.UrlType>();
            UrlType = CommonEnums.UrlType.Internal;
            Pages = _pageService.GetPossibleParents();
        }

        public PageDeleteModel(Page page)
            : this()
        {
            Id = page.Id;
            FriendlyUrl = page.FriendlyUrl;
            Title = page.Title;
            ReferencePages = _pageService.GetReferencePages(page).ToList()
                .Select(p => new PageDeleteItemModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    FriendlyUrl = p.FriendlyUrl.ToPageFriendlyUrl()
                }).ToList();
        }

        #region Public Properties

        public int Id { get; set; }

        public string FriendlyUrl { get; set; }

        public string Title { get; set; }

        public List<PageDeleteItemModel> ReferencePages { get; set; }

        [RequiredIf("ReplacePageId", null)]
        [LocalizedDisplayName("Page_Field_ReplaceUrl")]
        public string ReplaceUrl { get; set; }

        [RequiredIf("ReplaceUrl", null)]
        [LocalizedDisplayName("Page_Field_ReplacePageId")]
        public int? ReplacePageId { get; set; }

        public IEnumerable<SelectListItem> Pages { get; set; }

        public CommonEnums.UrlType UrlType { get; set; }

        public IEnumerable<CommonEnums.UrlType> UrlTypes { get; set; }

        #endregion
    }

    public class PageDeleteItemModel
    {
        #region Public Properties

        public int Id { get; set; }

        public string FriendlyUrl { get; set; }

        public string Title { get; set; }

        #endregion
    }
}
