using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.ProductOfInterests;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.ProductOfInterests
{
    [Register(Lifetime.PerInstance)]
    public interface IProductOfInterestService : IBaseService<ProductOfInterest>
    {
        /// <summary>
        /// Get product of interest select list
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetProductOfInterests();

        #region Grid Search

        JqGridSearchOut SearchProductOfInterests(JqSearchIn si);

        /// <summary>
        /// Export product of interests
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get Product Of Interest
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ProductOfInterestManageModel GetProductOfInterestManageModel(int? id = null);

        /// <summary>
        /// Update Product Of Interest
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveProductOfInterest(ProductOfInterestManageModel model);

        /// <summary>
        /// Delete Product Of Interest
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteProductOfInterest(int id);

        #endregion
    }
}