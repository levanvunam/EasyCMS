using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using System.Linq;

namespace EzCMS.Core.Services.FavouriteNavigations
{
    [Register(Lifetime.PerInstance)]
    public interface IFavouriteNavigationService : IBaseService<FavouriteNavigation>
    {
        /// <summary>
        /// Delete favourite navigations
        /// </summary>
        /// <param name="favouriteNavigations"></param>
        /// <returns></returns>
        ResponseModel Delete(IEnumerable<FavouriteNavigation> favouriteNavigations);

        /// <summary>
        /// Get current user favourite navigations
        /// </summary>
        /// <returns></returns>
        IQueryable<FavouriteNavigation> GetCurrentUserFavouriteNavigations();

        /// <summary>
        /// Add navigation to favourites
        /// </summary>
        /// <param name="navigationId">the navigation id to add</param>
        /// <returns></returns>
        ResponseModel AddToFavourites(string navigationId);

        /// <summary>
        /// Remove from favourites
        /// </summary>
        /// <param name="navigationId">the navigation id to remove</param>
        /// <returns></returns>
        ResponseModel RemoveFromFavourites(string navigationId);

        /// <summary>
        /// Move favourite navigation up
        /// </summary>
        /// <param name="favouriteNavigationId">the navigation id to move</param>
        /// <returns></returns>
        ResponseModel MoveUp(int favouriteNavigationId);

        /// <summary>
        /// Move favourite navigation down
        /// </summary>
        /// <param name="favouriteNavigationId">the navigation id to move</param>
        /// <returns></returns>
        ResponseModel MoveDown(int favouriteNavigationId);

        /// <summary>
        /// Delete favourite navigation
        /// </summary>
        /// <param name="id">the navigation id to delete</param>
        /// <returns></returns>
        ResponseModel Delete(int id);

        #region Grid Search

        /// <summary>
        /// Search the favourite Navigations
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchFavouriteNavigations(JqSearchIn si);

        /// <summary>
        /// Export the favourite Navigations
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion
    }
}