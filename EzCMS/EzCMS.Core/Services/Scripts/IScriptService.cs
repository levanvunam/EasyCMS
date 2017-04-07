using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Scripts;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Scripts
{
    [Register(Lifetime.PerInstance)]
    public interface IScriptService : IBaseService<Script>
    {
        #region Logs

        ScriptLogListingModel GetLogs(int id, int total = 0, int index = 1);

        #endregion

        #region Validation

        bool IsScriptNameExisted(int? scriptId, string name);

        #endregion

        /// <summary>
        /// Get url for script
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetScriptUrl(int? id);

        /// <summary>
        /// Get url for script
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetScriptUrl(string name);

        /// <summary>
        /// Get url for script
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        string GetScriptUrl(Script script);

        #region Grid Search

        JqGridSearchOut SearchScripts(JqSearchIn si);

        /// <summary>
        /// Export scripts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        ScriptManageModel GetScriptManageModelByLogId(int? logId);

        ScriptManageModel GetScriptManageModel(int? id = null);

        ScriptRenderModel GetScriptByName(string name);

        ResponseModel SaveScriptManageModel(ScriptManageModel model);

        ResponseModel DeleteScript(int id);

        ResponseModel CreateScript(Script script);

        #endregion
    }
}