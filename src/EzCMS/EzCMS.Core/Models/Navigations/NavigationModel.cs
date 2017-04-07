using System.Collections.Generic;

namespace EzCMS.Core.Models.Navigations
{
    public class NavigationModel
    {
        public NavigationModel()
        {
            SearchNavigations = new List<NavigationItemModel>();
            Navigations = new List<NavigationItemModel>();
        }

        #region Public Properties

        public List<NavigationItemModel> SearchNavigations { get; set; }

        public List<NavigationItemModel> Navigations { get; set; }

        #endregion
    }
}
