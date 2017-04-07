using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.NewsCategories;
using EzCMS.Core.Models.NewsCategories.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.NewsCategories
{
    [Register(Lifetime.PerInstance)]
    public interface INewsCategoryService : IBaseService<NewsCategory>
    {
        /// <summary>
        /// Get possible parent categories
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetPossibleParents(int? id = null);

        /// <summary>
        /// Get news categories
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetNewsCategories(int? newsId = null);

        /// <summary>
        /// Get news category details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        NewsCategoryDetailModel GetNewsCategoryDetailModel(int id);

        #region Grid Search

        /// <summary>
        /// Search news categories
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchNewsCategories(JqSearchIn si);

        /// <summary>
        /// Export news categories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        /// <summary>
        /// Search news categories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchNewsCategoriesOfNews(JqSearchIn si, int newsId);

        /// <summary>
        /// Export news categories of news
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        HSSFWorkbook ExportsNewsCategoriesOfNews(JqSearchIn si, GridExportMode gridExportMode, int newsId);

        /// <summary>
        /// Search news categories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="newsCategoryId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchChildrenNewsCategories(JqSearchIn si, int newsCategoryId);

        /// <summary>
        /// Export children news categories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="newsCategoryId"></param>
        /// <returns></returns>
        HSSFWorkbook ExportsChildrenNewsCategories(JqSearchIn si, GridExportMode gridExportMode, int newsCategoryId);

        #endregion

        #region Manage

        /// <summary>
        /// Get news category manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        NewsCategoryManageModel GetNewsCategoryManageModel(int? id = null);

        /// <summary>
        /// Save news category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveNewsCategoryManageModel(NewsCategoryManageModel model);

        /// <summary>
        /// Update news category details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateNewsCategoryData(XEditableModel model);

        /// <summary>
        /// Delete news category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteNewsCategory(int id);

        #endregion

        #region Validation

        /// <summary>
        /// Check if order is existed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        bool IsLevelOrderExisted(int? id, int? parentId, int order);

        /// <summary>
        /// Check if category name exists
        /// </summary>
        /// <param name="newsCategoryId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsNameExisted(int? newsCategoryId, string name);

        #endregion

        #region Widgets

        /// <summary>
        /// Get category listing
        /// </summary>
        /// <returns></returns>
        NewsCategoriesWidget GetCategoryListing();

        /// <summary>
        /// Get news category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        NewsCategoryWidget GetCategoryModel(int categoryId);

        #endregion
    }
}