using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.BackgroundTasks;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;

namespace EzCMS.Core.Services.BackgroundTasks
{
    [Register(Lifetime.PerInstance)]
    public interface IBackgroundTaskService : IEzBackgroundTaskService, IBaseService<BackgroundTask>
    {
        #region Grid Search

        /// <summary>
        /// Search background tasks
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchBackgroundTasks(JqSearchIn si);

        /// <summary>
        /// Export background tasks
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        /// <summary>
        /// Update background tasks
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        ResponseModel Update(BackgroundTask task);

        #endregion

        #region Manage

        /// <summary>
        /// Get task manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BackgroundTaskManageModel GetBackgroundTaskManageModel(int id);

        /// <summary>
        /// Save background task
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveBackgroundTask(BackgroundTaskManageModel model);

        #endregion

        #region Logs

        /// <summary>
        /// Get background task logs
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentDateLog"></param>
        /// <returns></returns>
        BackgroundTaskLogsModel GetLogs(int id, string currentDateLog);

        /// <summary>
        /// Get details of background task logs on a specific date
        /// </summary>
        /// <param name="backgroundTaskName"></param>
        /// <param name="dateLogs"></param>
        /// <returns></returns>
        List<string> GetLogsDetails(string backgroundTaskName, string dateLogs);

        #endregion
    }
}
