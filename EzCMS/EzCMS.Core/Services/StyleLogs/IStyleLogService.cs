using System;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.StyleLogs;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Services.StyleLogs
{
    [Register(Lifetime.PerInstance)]
    public interface IStyleLogService : IBaseService<StyleLog>
    {
        /// <summary>
        /// Save style log
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveStyleLog(StyleLogManageModel model);

        /// <summary>
        /// Delete style logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        ResponseModel DeleteStyleLogs(DateTime date);
    }
}