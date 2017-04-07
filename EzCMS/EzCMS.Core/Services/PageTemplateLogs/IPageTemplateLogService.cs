using System;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.PageTemplateLogs;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Services.PageTemplateLogs
{
    [Register(Lifetime.PerInstance)]
    public interface IPageTemplateLogService : IBaseService<PageTemplateLog>
    {
        /// <summary>
        /// Save page template log
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SavePageTemplateLog(PageTemplateLogManageModel model);

        /// <summary>
        /// Delete page template logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        ResponseModel DeletePageTemplateLogs(DateTime date);
    }
}