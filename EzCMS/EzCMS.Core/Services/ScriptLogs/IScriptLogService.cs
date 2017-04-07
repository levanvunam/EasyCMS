using System;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.ScriptLogs;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Services.ScriptLogs
{
    [Register(Lifetime.PerInstance)]
    public interface IScriptLogService : IBaseService<ScriptLog>
    {
        /// <summary>
        /// Save script log
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveScriptLog(ScriptLogManageModel model);

        /// <summary>
        /// Delete script logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        ResponseModel DeleteScriptLogs(DateTime date);
    }
}