using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Styles;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Styles
{
    [Register(Lifetime.PerInstance)]
    public interface IStyleService : IBaseService<Style>
    {
        #region Logs

        /// <summary>
        /// Get logs
        /// </summary>
        /// <param name="id"></param>
        /// <param name="total"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        StyleLogListingModel GetLogs(int id, int total = 0, int index = 1);

        #endregion

        #region Validation

        /// <summary>
        /// Check if style exists.
        /// </summary>
        /// <param name="styleId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsStyleNameExisted(int? styleId, string name);

        #endregion

        /// <summary>
        /// Get style render by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        StyleRenderModel GetStyleByName(string name);

        /// <summary>
        /// Get all included editor's styles
        /// </summary>
        /// <returns></returns>
        List<string> GetIncludedStyles();

        /// <summary>
        /// Generate style select list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetStyles(int? id = null);

        /// <summary>
        /// Get style by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Style GetByName(string name);

        /// <summary>
        /// Get url for style
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetStyleUrl(int? id);

        /// <summary>
        /// Get url for style
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetStyleUrl(string name);

        /// <summary>
        /// Get url for style
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        string GetStyleUrl(Style style);

        #region Grid Search

        /// <summary>
        /// Search the styles
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchStyles(JqSearchIn si);

        /// <summary>
        /// Export the styles
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get style manage model by log
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        StyleManageModel GetStyleManageModelByLogId(int? logId);

        /// <summary>
        /// Get style manage model by style id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        StyleManageModel GetStyleManageModel(int? id = null);

        /// <summary>
        /// Save style
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveStyleManageModel(StyleManageModel model);

        /// <summary>
        /// Delete style
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteStyle(int id);

        #endregion
    }
}