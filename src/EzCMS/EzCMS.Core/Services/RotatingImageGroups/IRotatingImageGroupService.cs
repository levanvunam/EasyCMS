using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.RotatingImageGroups;
using EzCMS.Core.Models.RotatingImageGroups.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.RotatingImageGroups
{
    [Register(Lifetime.PerInstance)]
    public interface IRotatingImageGroupService : IBaseService<RotatingImageGroup>
    {
        /// <summary>
        /// Get rotating image groups
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetRotatingImageGroups(int? groupId = null);

        /// <summary>
        /// Save group settings
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveGroupSettings(GroupManageSettingModel model);

        /// <summary>
        /// Get group settings
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        GroupManageSettingModel GetGroupManageSettingModel(int id);

        /// <summary>
        /// Get rotating image widget
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RotatingImagesWidget GetRotatingImageWidget(int id);

        /// <summary>
        /// Sort images
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SortImages(GroupImageSortingModel model);

        /// <summary>
        /// Delete rotating image group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteRotatingImageGroup(int id);

        #region Grid Search

        /// <summary>
        /// Search rotating image groups
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchRotatingImageGroups(JqSearchIn si);

        /// <summary>
        /// Export rotating image groups
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get rotating image group manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RotatingImageGroupManageModel GetRotatingImageGroupManageModel(int? id = null);

        /// <summary>
        /// Save rotating image group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveRotatingImageGroup(RotatingImageGroupManageModel model);

        #endregion
    }
}