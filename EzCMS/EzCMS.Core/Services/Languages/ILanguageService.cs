using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Languages;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Languages
{
    [Register(Lifetime.PerInstance)]
    public interface ILanguageService : IBaseService<Language>
    {
        #region Validation

        /// <summary>
        /// Check key is existed or not
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsKeyExisted(int? languageId, string key);

        #endregion

        /// <summary>
        /// Get language by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Language GetByKey(string key);

        /// <summary>
        /// Get default language
        /// </summary>
        /// <returns></returns>
        Language GetDefault();

        /// <summary>
        /// Get list languages
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetLanguages(string key = null);

        #region Grid

        /// <summary>
        /// Search languages
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchLanguages(JqSearchIn si);

        /// <summary>
        /// Export languages
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get language manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LanguageManageModel GetLanguageManageModel(int? id = null);

        /// <summary>
        /// Save language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveLanguage(LanguageManageModel model);

        #endregion
    }
}