using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.RssFeeds;
using EzCMS.Core.Models.RssFeeds.Widgets;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.RssFeeds
{
    [Register(Lifetime.PerInstance)]
    public interface IRssFeedService : IBaseService<RssFeed>
    {
        #region Widget

        /// <summary>
        /// Get RSS feed to display in widget
        /// </summary>
        /// <param name="rssFeedId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        RssFeedWidget GetRssFeedWidget(int rssFeedId, int? number);

        #endregion

        /// <summary>
        /// Get RSS feeds for select list
        /// </summary>
        /// <param name="rssType"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetRssFeeds(RssFeedEnums.RssType? rssType);

        #region Validation

        /// <summary>
        /// Check if RSS name exists
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsNameExisted(int? id, string name);

        #endregion

        #region Grid Search

        /// <summary>
        /// Search feeds
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchRssFeed(JqSearchIn si);

        /// <summary>
        /// Export rss feeds
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get RSS feed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RssFeedManageModel GetRssFeedManageModel(int? id = null);

        /// <summary>
        /// Save RSS feed
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveRssFeed(RssFeedManageModel model);

        /// <summary>
        /// Delete RSS feed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteRssFeed(int id);

        #endregion

        #region Details

        /// <summary>
        /// Get RSS details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RssFeedDetailModel GetRssFeedDetailModel(int id);

        /// <summary>
        /// Update RSS feed
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateRssFeedData(XEditableModel model);

        #endregion
    }
}