using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.SlideInHelps;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.SlideInHelps
{
    [Register(Lifetime.PerInstance)]
    public interface ISlideInHelpService : IBaseService<SlideInHelp>
    {
        #region Initialize

        /// <summary>
        /// Refresh dictionary
        /// </summary>
        void RefreshDictionary();

        #endregion

        #region Fetch data

        /// <summary>
        /// Gets the localized string from a text key, if the value is not available then add the default value to the
        /// dictionary
        /// </summary>
        /// <param name="textKey">Key of the string to get localized</param>
        /// <returns></returns>
        SlideInHelpDictionaryItem GetSlideInHelp(string textKey);

        #endregion

        #region Validation

        /// <summary>
        /// Check if slide in help key is unique or not
        /// </summary>
        /// <param name="key"></param>
        /// <param name="languageId"></param>
        /// <param name="slideInHelpId"></param>
        /// <returns></returns>
        bool IsKeyExisted(string key, int languageId, int? slideInHelpId = null);

        #endregion

        #region Grid

        /// <summary>
        /// Search the slide in helper
        /// </summary>
        /// <param name="si"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchSlideInHelps(JqSearchIn si, int languageId);

        /// <summary>
        /// Export the slide in helps
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int languageId);

        #endregion

        #region Manage

        /// <summary>
        /// Get slide in help manage model
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        SlideInHelpManageModel GetSlideInHelpManageModel(int? languageId, int? id = null);

        /// <summary>
        /// Save slide in help
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveSlideInHelpManageModel(SlideInHelpManageModel model);

        /// <summary>
        /// Change slide in help status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        ResponseModel ChangeSlideInHelpStatus(int id, bool status);

        /// <summary>
        /// Update slide in help
        /// </summary>
        /// <param name="slideInHelp"></param>
        /// <returns></returns>
        ResponseModel Update(SlideInHelp slideInHelp);

        /// <summary>
        /// Insert slide in help
        /// </summary>
        /// <param name="slideInHelp"></param>
        /// <returns></returns>
        ResponseModel Insert(SlideInHelp slideInHelp);

        #endregion
    }
}