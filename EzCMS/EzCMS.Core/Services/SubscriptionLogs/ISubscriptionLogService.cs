using System;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.SubscriptionLogs;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.SubscriptionLogs
{
    [Register(Lifetime.PerInstance)]
    public interface ISubscriptionLogService : IBaseService<SubscriptionLog>
    {
        #region Manage

        ResponseModel SaveSubscriptionLog(SubscriptionLogManageModel model);

        #endregion

        /// <summary>
        /// Get all logs from start time
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="module"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IQueryable<SubscriptionLog> GetLogs(
            SubscriptionEnums.SubscriptionType? type = null,
            DateTime? startTime = null, SubscriptionEnums.SubscriptionModule? module = null);

        /// <summary>
        /// Deactive log
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel DeactiveSubscriptionLog(SubscriptionLog model);

        /// <summary>
        /// Disabled Nightly Subscription
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel DisabledSubscriptionNighly(SubscriptionLog model);

        /// <summary>
        /// Disable Instantly Subscription after sent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel DisabledSubscriptionDirectly(SubscriptionLog model);

        /// <summary>
        /// Delete subdcription logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        ResponseModel DeleteSubscriptionLogs(DateTime date);

        #region Grid Search

        /// <summary>
        /// Search the subscription log
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchSubscriptionLogs(JqSearchIn si);

        /// <summary>
        /// Export the subscription log
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion
    }
}