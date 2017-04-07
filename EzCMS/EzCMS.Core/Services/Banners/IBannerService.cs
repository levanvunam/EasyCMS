using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Banners;
using EzCMS.Core.Models.Banners.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Banners
{
    [Register(Lifetime.PerInstance)]
    public interface IBannerService : IBaseService<Banner>
    {
        /// <summary>
        /// Get banner select list
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetBanners();

        /// <summary>
        /// Delete Banner
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteBanner(int id);

        /// <summary>
        /// Get banner widget
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BannerWidget GetBannerWidget(int id);

        #region Grid Search

        /// <summary>
        /// Search banners
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchGrid(JqSearchIn si);

        /// <summary>
        /// Export banner
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get banner manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BannerManageModel GetBannerManageModel(int? id = null);

        /// <summary>
        /// Save banner
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveBanner(BannerManageModel model);

        #endregion
    }
}