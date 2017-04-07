using System;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.PageLogs;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Services.PageLogs
{
    [Register(Lifetime.PerInstance)]
    public interface IPageLogService : IBaseService<PageLog>
    {
        #region Grid Search

        JqGridSearchOut SearchPageLogs(JqSearchIn si);

        #endregion

        /// <summary>
        /// Save current page to audit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SavePageLog(PageLogManageModel model);

        /// <summary>
        /// Delete page logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        ResponseModel DeletePageLogs(DateTime date);
    }
}