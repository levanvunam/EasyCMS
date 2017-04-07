using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Locale.Services;
using Ez.Framework.Core.Mvc.Models.Editable;
using EzCMS.Core.Models.LocalizedResources;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Localizes
{
    [Register(Lifetime.PerInstance)]
    public interface IEzCMSLocalizedResourceService : ILocalizedResourceService
    {
        #region Initialize

        /// <summary>
        /// Refresh dictionary
        /// </summary>
        void RefreshDictionary();

        #endregion

        #region Validation

        /// <summary>
        /// Check text key is existed or not
        /// </summary>
        /// <param name="localizedResourceId"></param>
        /// <param name="languageId"></param>
        /// <param name="textKey"></param>
        /// <returns></returns>
        bool IsTextKeyExisted(int? localizedResourceId, int languageId, string textKey);

        #endregion

        /// <summary>
        /// Update localize resouce
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateLocalizedResource(XEditableModel model);

        #region Grid

        /// <summary>
        /// Search localized resources
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchLocalizedResources(JqSearchIn si, LocalizedResourceSearchModel model);

        /// <summary>
        /// Exports
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, LocalizedResourceSearchModel model);

        #endregion

        #region Manage

        /// <summary>
        /// Get localized resources manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LocalizedResourceManageModel GetLocalizedResourceManageModel(int? id = null);

        /// <summary>
        /// Save localized resource
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveLocalizedResource(LocalizedResourceManageModel model);

        /// <summary>
        /// Delete localized resource
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteLocalizedResource(int id);

        #endregion
    }
}