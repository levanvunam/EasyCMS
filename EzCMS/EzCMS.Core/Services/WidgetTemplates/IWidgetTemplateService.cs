using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.WidgetTemplates;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EzCMS.Core.Services.WidgetTemplates
{
    [Register(Lifetime.PerInstance)]
    public interface IWidgetTemplateService : IBaseService<WidgetTemplate>
    {
        #region Logs

        /// <summary>
        /// Get template logs
        /// </summary>
        /// <param name="id"></param>
        /// <param name="total"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        WidgetTemplateLogListingModel GetLogs(int id, int total = 0, int index = 1);

        #endregion

        #region Validation

        /// <summary>
        /// Check if template name is existed
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsTemplateNameExisted(int? templateId, string name);

        #endregion

        /// <summary>
        /// Get template render model by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        WidgetTemplateRenderModel GetTemplateByName(string name);

        /// <summary>
        /// Get template by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        WidgetTemplate GetTemplate(string name);

        /// <summary>
        /// Get all available templates of widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetTemplatesOfWidget(string widget);

        /// <summary>
        /// Parse template
        /// </summary>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <param name="viewbag"></param>
        /// <returns></returns>
        string ParseTemplate(WidgetTemplateRenderModel widgetTemplate, dynamic model, dynamic viewbag = null);

        /// <summary>
        /// Generate full template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string GetFullTemplate(WidgetTemplateManageModel model);

        #region Grid Search

        /// <summary>
        /// Search the template
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchTemplates(JqSearchIn si, WidgetTemplateSearchModel model);

        /// <summary>
        /// Export the template
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, WidgetTemplateSearchModel model);

        #endregion

        #region Manage

        /// <summary>
        /// Get template manage model by log id
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        WidgetTemplateManageModel GetTemplateManageModelByLogId(int? logId);

        /// <summary>
        /// Get template manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        WidgetTemplateManageModel GetTemplateManageModel(int? id = null);

        /// <summary>
        /// Get template manage model by widget
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        WidgetTemplateManageModel GetTemplateManageModel(string widget, int? templateId);

        /// <summary>
        /// Save template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveTemplateManageModel(WidgetTemplateManageModel model);

        /// <summary>
        /// Delete template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteTemplate(int id);

        /// <summary>
        /// Create template
        /// </summary>
        /// <param name="widgetTemplate"></param>
        /// <returns></returns>
        ResponseModel CreateTemplate(WidgetTemplate widgetTemplate);

        #endregion
    }
}