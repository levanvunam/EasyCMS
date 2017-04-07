using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.NewsCategories;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.News
{
    public class NewsSearchModel
    {
        #region Constructors

        public NewsSearchModel()
        {
            var newsCategoryService = HostContainer.GetInstance<INewsCategoryService>();
            NewsCategories = newsCategoryService.GetNewsCategories();
        }

        #endregion

        #region Public Properties

        public string Keyword { get; set; }

        public int? NewsCategoryId { get; set; }

        public IEnumerable<SelectListItem> NewsCategories { get; set; }

        #endregion
    }
}
