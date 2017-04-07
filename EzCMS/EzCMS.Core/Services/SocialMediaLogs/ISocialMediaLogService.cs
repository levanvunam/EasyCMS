using System;
using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.SocialMediaLogs
{
    [Register(Lifetime.PerInstance)]
    public interface ISocialMediaLogService : IBaseService<SocialMediaLog>
    {
        /// <summary>
        /// Insert social media log
        /// </summary>
        /// <param name="socialMediaLog"></param>
        /// <returns></returns>
        ResponseModel Insert(SocialMediaLog socialMediaLog);

        /// <summary>
        /// Save social media log
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveSocialMediaLog(SocialMediaLog model);

        /// <summary>
        /// Delete social media logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        ResponseModel DeleteSocialMediaLogs(DateTime date);

        /// <summary>
        /// Delete social media logs
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        ResponseModel DeleteSocialMediaLogs(IEnumerable<SocialMediaLog> logs);

        #region Grid Search

        /// <summary>
        /// Search social media logs
        /// </summary>
        /// <param name="si"></param>
        /// <param name="socialMediaTokenId"></param>
        /// <param name="socialMediaId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchSocialMediaLogs(JqSearchIn si, int? socialMediaTokenId, int? socialMediaId);

        /// <summary>
        /// Export social media logs
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="socialMediaTokenId"></param>
        /// <param name="socialMediaId"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? socialMediaTokenId, int? socialMediaId);

        #endregion
    }
}