using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.News;
using EzCMS.Core.Models.News.Widgets;
using EzCMS.Entity.Core.Enums;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EzCMS.Core.Services.News
{
    [Register(Lifetime.PerInstance)]
    public interface INewsService : IBaseService<Entity.Entities.Models.News>
    {
        #region Validation

        /// <summary>
        /// Check if title exists or not
        /// </summary>
        /// <param name="newsId"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        bool IsTitleExisted(int? newsId, string title);

        #endregion

        /// <summary>
        /// Setup news tracking
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        ResponseModel SetupNewsTracking(int newsId);

        /// <summary>
        /// Get viewable news
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entity.Entities.Models.News GetViewableNews(int id);

        /// <summary>
        /// Get news select list
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetNewsList();

        #region Grid Search

        /// <summary>
        /// Search the news
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchNews(JqSearchIn si, NewsSearchModel model);

        /// <summary>
        /// Export news
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, NewsSearchModel model);

        #endregion

        #region Manage

        /// <summary>
        /// Get news manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        NewsManageModel GetNewsManageModel(int? id = null);

        /// <summary>
        /// Save news
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveNews(NewsManageModel model);

        /// <summary>
        /// Delete News
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteNews(int id);

        /// <summary>
        /// Remove relationship between News and News Category
        /// </summary>
        /// <param name="newsId"></param>
        /// <param name="newsCategoryId"></param>
        /// <returns></returns>
        ResponseModel DeleteNewsNewsCategoryMapping(int newsId, int newsCategoryId);

        #endregion

        #region Widgets

        /// <summary>
        /// Get news widget by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        NewsWidget GetNewsDetails(int id);

        /// <summary>
        /// Get news listing with paging
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <param name="newsType"></param>
        /// <param name="categoryId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        NewsListingWidget GetNewsListing(int index, int pageSize, int total, NewsEnums.NewsType newsType,
            int? categoryId, NewsEnums.NewsStatus status = NewsEnums.NewsStatus.Active);

        /// <summary>
        /// Get news belong to category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        List<NewsWidget> GetNewsOfCategory(int categoryId);

        #endregion

        #region Details

        /// <summary>
        /// Get news details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        NewsDetailModel GetNewsDetailModel(int id);

        /// <summary>
        /// Update News
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateNewsData(XEditableModel model);

        #endregion
    }
}