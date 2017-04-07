using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.WidgetTemplateLogs;
using EzCMS.Entity.Entities.Models;
using System;

namespace EzCMS.Core.Services.WidgetTemplateLogs
{
    [Register(Lifetime.PerInstance)]
    public interface IWidgetTemplateLogService : IBaseService<WidgetTemplateLog>
    {
        /// <summary>
        /// Save template log
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveTemplateLog(WidgetTemplateLogManageModel model);

        /// <summary>
        /// Delete template logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        ResponseModel DeleteTemplateLogs(DateTime date);
    }
}