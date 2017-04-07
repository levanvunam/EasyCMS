using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.NotificationTemplates;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.NotificationTemplates
{
    [Register(Lifetime.PerInstance)]
    public interface INotificationTemplateService : IBaseService<NotificationTemplate>
    {
        #region Base

        NotificationTemplate GetByName(string name);

        #endregion

        #region Validation

        /// <summary>
        /// Check if notification template exists
        /// </summary>
        /// <param name="id">the template id</param>
        /// <param name="name">the template name</param>
        /// <returns></returns>
        bool IsNameExisted(int? id, string name);

        #endregion

        /// <summary>
        /// Get notification email model assembly qualified name by module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        string GetNotificationEmailModelAssemblyName(NotificationEnums.NotificationModule module);

        #region Grid

        /// <summary>
        /// Search the notification templates
        /// </summary>
        /// <returns></returns>
        JqGridSearchOut SearchNotificationTemplates(JqSearchIn si);

        /// <summary>
        /// Export notification templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Parse Email

        /// <summary>
        /// Parse the notification
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        NotificationTemplateResponseModel ParseNotification<TModel>(int? id, TModel model);

        /// <summary>
        /// Parse the notification
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        NotificationTemplateResponseModel ParseNotification<TModel>(NotificationTemplate template, TModel model);

        #endregion

        #region Manage

        /// <summary>
        /// Get notification template detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        NotificationTemplateDetailModel GetNotificationTemplateDetailModel(int? id = null);

        /// <summary>
        /// Update notification template data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateNotificationTemplateData(XEditableModel model);

        /// <summary>
        /// Get notification template manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        NotificationTemplateManageModel GetNotificationTemplateManageModel(int? id = null);

        /// <summary>
        /// Get default notification template by module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        NotificationTemplate GetDefaultNotificationTemplate(NotificationEnums.NotificationModule module);

        /// <summary>
        /// Get all active notification templates of a module
        /// </summary>
        /// <param name="module"></param>
        /// <param name="selectDefault"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetNotificationTemplates(NotificationEnums.NotificationModule module,
            bool selectDefault = false);

        /// <summary>
        /// Save notification template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveNotificationTemplate(NotificationTemplateManageModel model);

        /// <summary>
        /// Delete the notification template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteNotificationTemplate(int id);

        #endregion
    }
}