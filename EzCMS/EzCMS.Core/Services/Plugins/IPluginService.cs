using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using EzCMS.Core.Models.Plugins;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Plugins
{
    [Register(Lifetime.PerInstance)]
    public interface IPluginService
    {
        #region Base

        PluginInformationModel GetByName(string name);

        IEnumerable<PluginInformationModel> GetAll();

        #endregion

        #region Grid Search

        JqGridSearchOut SearchPlugins(JqSearchIn si);

        /// <summary>
        /// Export Plugins
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        PluginInformationModel GetPluginManageModel(string name);

        ResponseModel SavePluginManageModel(PluginInformationModel model);

        #endregion
    }
}