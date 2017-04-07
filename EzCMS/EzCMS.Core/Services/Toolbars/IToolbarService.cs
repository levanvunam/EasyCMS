using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Toolbars;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Toolbars
{
    [Register(Lifetime.PerInstance)]
    public interface IToolbarService : IBaseService<Toolbar>
    {
        #region Validation

        /// <summary>
        /// Check if toolbar exists
        /// </summary>
        /// <param name="toolbarId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsToolbarExisted(int? toolbarId, string name);

        #endregion

        /// <summary>
        /// Get list of toolbar
        /// </summary>
        /// <param name="toolbarId"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetToolbars(int? toolbarId = null);

        /// <summary>
        /// Get toolbar for current user
        /// </summary>
        /// <returns></returns>
        ToolbarRenderModel GetCurrentUserToolbar();

        #region Grid Search

        /// <summary>
        /// Search the toolbars.
        /// </summary>
        /// <returns></returns>
        JqGridSearchOut SearchToolbars(JqSearchIn si);

        /// <summary>
        /// Export toolbars
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get toolbar manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ToolbarManageModel GetToolbarManageModel(int? id = null);

        /// <summary>
        /// Save toolbar
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveToolbar(ToolbarManageModel model);

        /// <summary>
        /// Delete toolbar by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteToolbar(int id);

        #endregion

        #region Details

        /// <summary>
        /// Get toolbar detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ToolbarDetailModel GetToolbarDetailModel(int id);

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateToolbarData(XEditableModel model);

        #endregion
    }
}