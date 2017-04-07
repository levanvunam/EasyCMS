using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.PageTemplates;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.PageTemplates
{
    [Register(Lifetime.PerInstance)]
    public interface IPageTemplateService : IBaseService<PageTemplate>
    {
        #region Initialize

        /// <summary>
        /// Reset cache name for all hierarchies templates of page
        /// If null then reset all the application
        /// </summary>
        /// <param name="pageTemplateId"></param>
        void Initialize(int? pageTemplateId = null);

        #endregion

        #region Logs

        /// <summary>
        /// Get page template logs
        /// </summary>
        /// <param name="id"></param>
        /// <param name="total"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        PageTemplateLogListingModel GetLogs(int id, int total = 0, int index = 1);

        #endregion

        /// <summary>
        /// Get possible parent Navigation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetPossibleParents(int? id = null);

        /// <summary>
        /// Get children page templates by parent id
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        List<PageTemplate> GetPageTemplates(int? parentId = null);

        /// <summary>
        /// Get page template select list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetPageTemplateSelectList(int? id = null);

        /// <summary>
        /// Get page templates for file template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetPageTemplateSelectListForFileTemplate(int? id = null);

        /// <summary>
        /// Get page template for virtual path provider
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        PageTemplate FindTemplate(string filePath);

        #region Grid Search

        /// <summary>
        /// Search the page templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="pageTemplateId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchPageTemplates(JqSearchIn si, int? pageTemplateId);

        /// <summary>
        /// Exports page templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="pageTemplateI"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? pageTemplateI);

        #endregion

        #region Manage

        /// <summary>
        /// Get page template manage model for creating/editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PageTemplateManageModel GetTemplateManageModel(int? id = null);

        /// <summary>
        /// Get page template manage model from log
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        PageTemplateManageModel GetTemplateManageModelByLogId(int? logId = null);

        /// <summary>
        /// Save page template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SavePageTemplate(PageTemplateManageModel model);

        /// <summary>
        /// Delete the page template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeletePageTemplate(int id);

        #endregion

        #region Validation

        /// <summary>
        /// Check if page template exists
        /// </summary>
        /// <param name="pageTemplateId">the template id</param>
        /// <param name="name">the template name</param>
        /// <returns></returns>
        bool IsPageTemplateNameExisted(int? pageTemplateId, string name);

        /// <summary>
        /// Check if page template existed for virtual path provider
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        bool IsPageTemplateExisted(string filePath);

        #endregion

        #region Details

        /// <summary>
        /// Get page template details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PageTemplateDetailModel GetPageTemplateDetailModel(int id);

        /// <summary>
        /// Update page template data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdatePageTemplateData(XEditableModel model);

        #endregion
    }
}