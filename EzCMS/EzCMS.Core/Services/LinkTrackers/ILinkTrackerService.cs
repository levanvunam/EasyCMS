using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.LinkTrackers;
using EzCMS.Core.Models.LinkTrackers.MonthlyClickThrough;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.LinkTrackers
{
    [Register(Lifetime.PerInstance)]
    public interface ILinkTrackerService : IBaseService<LinkTracker>
    {
        #region Validation

        /// <summary>
        /// Check if link tracker exists
        /// </summary>
        /// <param name="id">the link tracker id</param>
        /// <param name="name">the link tracker name</param>
        /// <returns></returns>
        bool IsNameExisted(int? id, string name);

        #endregion

        #region Link Tracker action

        /// <summary>
        /// Handle link tracker actions
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel TriggerLinkTrackerAction(int id);

        #endregion

        #region Grid

        /// <summary>
        /// Search the link trackers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        JqGridSearchOut SearchLinkTrackers(JqSearchIn si, LinkTrackerSearchModel searchModel);

        /// <summary>
        /// Export link trackers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, LinkTrackerSearchModel searchModel);

        /// <summary>
        /// Search the link trackers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchLinkTrackersByPage(JqSearchIn si, int pageId);

        /// <summary>
        /// Export link trackers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        HSSFWorkbook ExportsLinkTrackersByPage(JqSearchIn si, GridExportMode gridExportMode, int pageId);

        #endregion

        #region Manage

        /// <summary>
        /// Get link tracker detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LinkTrackerDetailModel GetLinkTrackerDetailModel(int? id = null);

        /// <summary>
        /// Get link tracker manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LinkTrackerManageModel GetLinkTrackerManageModel(int? id = null);

        /// <summary>
        /// Save link tracker
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveLinkTracker(LinkTrackerManageModel model);

        /// <summary>
        /// Delete the link tracker
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteLinkTracker(int id);

        /// <summary>
        /// Update link tracker data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateLinkTrackerData(XEditableModel model);

        #endregion

        #region Monthly Click Through

        /// <summary>
        /// Search the link trackers for monthly click through
        /// </summary>
        /// <returns></returns>
        JqGridSearchOut SearchLinkTrackers(JqSearchIn si, LinkTrackerMonthlyClickThroughSearchModel searchModel);

        /// <summary>
        /// Export link tracker monthly click through
        /// </summary>
        /// <returns></returns>
        HSSFWorkbook Exports(LinkTrackerMonthlyClickThroughSearchModel searchModel);

        #endregion
    }
}