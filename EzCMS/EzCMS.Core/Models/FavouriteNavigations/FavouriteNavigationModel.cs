using Ez.Framework.Models;

namespace EzCMS.Core.Models.FavouriteNavigations
{
    public class FavouriteNavigationModel : BaseGridModel
    {
        #region Public Properties

        public int UserId { get; set; }

        public string NavigationId { get; set; }

        #endregion
    }
}
