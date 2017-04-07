using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.ClientNavigations;
using EzCMS.Core.Models.ClientNavigations.Widgets;
using EzCMS.Core.Models.Pages.Sitemap;
using EzCMS.Core.Services.Tree;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.ClientNavigations
{
    [Register(Lifetime.PerInstance)]
    public interface IClientNavigationService : IBaseService<ClientNavigation>
    {
        /// <summary>
        /// Get possible parents for client Navigation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetPossibleParents(int? id = null);

        #region Grid Search

        /// <summary>
        /// Search client Navigations
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchClientNavigations(JqSearchIn si);

        /// <summary>
        /// Export client Navigations
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get client Navigation manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ClientNavigationManageModel GetClientNavigationManageModel(int? id = null);

        /// <summary>
        /// Save client Navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveClientNavigationManageModel(ClientNavigationManageModel model);

        /// <summary>
        /// Get relative Navigations
        /// </summary>
        /// <param name="position"></param>
        /// <param name="relativeClientNavigationId"></param>
        /// <param name="clientNavigationId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetRelativeNavigations(out int position, out int relativeClientNavigationId,
            int? clientNavigationId = null, int? parentId = null);

        /// <summary>
        /// Get relative Navigations
        /// </summary>
        /// <param name="NavigationId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetRelativeNavigations(int? NavigationId = null, int? parentId = null);

        /// <summary>
        /// Delete client Navigation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteNavigation(int id);

        #endregion

        #region Widgets

        #region Sitemap

        /// <summary>
        /// Generate site map data
        /// </summary>
        /// <param name="pageRootId"></param>
        /// <returns></returns>
        List<ITree<NavigationNodeModel>> GenerateSiteMap(int? pageRootId = null);

        /// <summary>
        /// Generate Google site map for site
        /// </summary>
        /// <returns></returns>
        GoogleSiteMap GenerateGoogleSiteMap();

        #endregion

        #region Navigations

        /// <summary>
        /// Generate Navigations
        /// </summary>
        /// <param name="pageRootId"></param>
        /// <param name="includeRoot"></param>
        /// <returns></returns>
        List<ITree<NavigationNodeModel>> GenerateNavigations(int? pageRootId = null, bool includeRoot = false);

        #endregion

        #endregion
    }
}