using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using Ez.Framework.Core.SiteSettings.Services;
using EzCMS.Core.Models.SiteSettings;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EzCMS.Core.Services.SiteSettings
{
    [Register(Lifetime.PerInstance)]
    public interface ISiteSettingService : IEzSiteSettingService, IBaseService<SiteSetting>
    {
        /// <summary>
        /// Get setting model by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        SiteSettingModel GetSetting(string key);

        /// <summary>
        /// Get by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        SiteSetting GetByKey(string key);

        IEnumerable<SelectListItem> GetSettingTypes();

        #region Grid

        /// <summary>
        /// Search site settings
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchSiteSettings(JqSearchIn si, SiteSettingSearchModel model);

        /// <summary>
        /// Export site settings
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, SiteSettingSearchModel model);

        #endregion

        #region Manage

        /// <summary>
        /// Get site setting manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SiteSettingManageModel GetSettingManageModel(int? id = null);

        /// <summary>
        /// Get site setting manage model by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        SiteSettingManageModel GetSettingManageModel(string name);

        /// <summary>
        /// Save site setting
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveSettingManageModel(SiteSettingManageModel model);

        #endregion
    }
}