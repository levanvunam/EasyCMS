using System;
using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.ProtectedDocumentLogs;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.ProtectedDocumentLogs
{
    [Register(Lifetime.PerInstance)]
    public interface IProtectedDocumentLogService : IBaseService<ProtectedDocumentLog>
    {
        #region Manage

        ResponseModel SaveProtectedDocumentLog(string path);

        #endregion

        IQueryable<ProtectedDocumentLogModel> GetCurrentUserLogs();

        List<ProtectedDocumentLogModel> GetCurrentUserRelatedLogs(string path);

        /// <summary>
        /// Delete protected document logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        ResponseModel DeleteProtectedDocumentLogs(DateTime date);

        #region Grid Search

        JqGridSearchOut SearchProtectedDocumentLogs(JqSearchIn si);
        HSSFWorkbook Exports();

        #endregion
    }
}