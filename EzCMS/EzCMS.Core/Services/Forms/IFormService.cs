using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Forms.Render;
using EzCMS.Core.Models.Forms.Setup;
using EzCMS.Core.Models.Forms.Setup.BuildFormSetup;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Forms
{
    [Register(Lifetime.PerInstance)]
    public interface IFormService : IBaseService<Form>
    {
        /// <summary>
        /// Load iframe form
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="formSubmitted"></param>
        /// <returns></returns>
        FormIframeLoaderModel LoadForm(string formId, bool formSubmitted = false);

        /// <summary>
        /// Delete form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteForm(int id);

        /// <summary>
        /// Get Form select list
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetForms();

        #region Grid Search

        /// <summary>
        /// Search forms
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchForms(JqSearchIn si);

        /// <summary>
        /// Export forms
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Setup

        #region Build form

        /// <summary>
        /// Get form setup model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BuildFormSetupModel GetBuildFormSetupModel(int? id);

        /// <summary>
        /// Save form setup model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveFormSetupModel(BuildFormSetupModel model);

        /// <summary>
        /// Get form configuration
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FormConfigurationModel GetConfigurations(int? id);

        /// <summary>
        /// Get available form templates
        /// </summary>
        /// <returns></returns>
        string GetFormTemplates();

        #endregion

        #region Configurations

        ConfigurationSetupModel GetConfigurationSetupModel(int id);

        ResponseModel SaveConfiguration(ConfigurationSetupModel model);

        #endregion

        #region Embedded

        EmbeddedScriptSetupModel GetEmbeddedScriptModel(int id);

        #endregion

        #region Preview

        ResponseModel SaveDraft(int? id, string content);

        FormIframeLoaderModel LoadPreview(int? id, string token);

        #endregion

        #endregion

        #region Form Apis

        /// <summary>
        /// Load form loader script
        /// </summary>
        /// <param name="formEncryptedId"></param>
        /// <returns></returns>
        string LoadLoaderScript(string formEncryptedId);

        /// <summary>
        /// Load loader script
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        string LoadLoaderScript(int formId);

        /// <summary>
        /// Load embeded script
        /// </summary>
        /// <param name="formEncryptedId"></param>
        /// <returns></returns>
        string LoadEmbeddedScript(string formEncryptedId);

        /// <summary>
        /// Load embedded script
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        string LoadEmbeddedScript(int formId);

        /// <summary>
        /// Submit form
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="contact"></param>
        /// <param name="communication"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        ResponseModel SubmitForm(string formId, Contact contact, ContactCommunication communication,
            NameValueCollection collection);

        /// <summary>
        /// Parse form
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        string ParseForm(string name, string content);

        #endregion
    }
}