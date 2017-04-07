using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Notices;
using EzCMS.Core.Models.Notices.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Notices
{
    [Register(Lifetime.PerInstance)]
    public interface INoticeService : IBaseService<Notice>
    {
        #region Widget

        NoticeboardWidget GetNoticesWidgets(int noticeTypeId, int number);

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the notice
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchNotices(JqSearchIn si, NoticeSearchModel model);

        /// <summary>
        /// Export the notices
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, NoticeSearchModel model);

        #endregion

        #region Manage

        /// <summary>
        /// Get notice manage model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="noticeTypeId"></param>
        /// <returns></returns>
        NoticeManageModel GetNoticeManageModel(int? id = null, int? noticeTypeId = null);

        /// <summary>
        /// Save notice
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveNotice(NoticeManageModel model);

        /// <summary>
        /// Delete notice
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteNotice(int id);

        #endregion

        #region Details

        /// <summary>
        /// Get notice details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        NoticeDetailModel GetNoticeDetailModel(int id);

        /// <summary>
        /// Update notice data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateNoticeData(XEditableModel model);

        #endregion
    }
}