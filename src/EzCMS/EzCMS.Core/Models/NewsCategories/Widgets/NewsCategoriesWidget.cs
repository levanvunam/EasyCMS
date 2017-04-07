using System.Collections.Generic;

namespace EzCMS.Core.Models.NewsCategories.Widgets
{
    public class NewsCategoriesWidget
    {
        public NewsCategoriesWidget()
        {
            Categories = new List<NewsCategoryWidget>();
        }

        #region Public Properties

        public List<NewsCategoryWidget> Categories { get; set; }

        #endregion
    }
}
