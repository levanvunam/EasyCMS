using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.RemoteAuthentications;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.RemoteAuthentications
{
    [Register(Lifetime.PerInstance)]
    public interface IRemoteAuthenticationService : IBaseService<RemoteAuthentication>
    {
        List<RemoteAuthenticationModel> GetActiveRemoteServices();

        #region Grid Search

        JqGridSearchOut SearchRemoteAuthentications(JqSearchIn si);

        /// <summary>
        /// Export remote authenticate configurations
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        RemoteAuthenticationManageModel GetRemoteAuthenticationManageModel(int? id = null);

        ResponseModel SaveRemoteAuthentication(RemoteAuthenticationManageModel model);

        #endregion
    }
}