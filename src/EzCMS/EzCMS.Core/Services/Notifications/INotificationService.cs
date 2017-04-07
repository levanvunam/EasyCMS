using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Models.Notifications;
using EzCMS.Core.Models.Notifications.Setup;
using EzCMS.Core.Models.Notifications.Setup.NotificationSetup;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Notifications
{
    [Register(Lifetime.PerInstance)]
    public interface INotificationService : IBaseService<Notification>
    {
        #region Validation

        ResponseModel IsNotificationSetupFilter(int id);

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the notifications
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchNotifications(JqSearchIn si);

        /// <summary>
        /// Export Notifications
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        /// <summary>
        /// Search the notified contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchNotifiedContacts(JqSearchIn si, int notificationId);

        /// <summary>
        /// Export notified contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        HSSFWorkbook ExportsNotifiedContacts(JqSearchIn si, GridExportMode gridExportMode, int notificationId);

        #endregion

        #region Manage

        /// <summary>
        /// Get notification detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        NotificationDetailModel GetNotificationDetailModel(int? id = null);

        /// <summary>
        /// Get notifications
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        IQueryable<Notification> GetNotifications(NotificationEnums.NotificationModule? module = null);

        /// <summary>
        /// Get notification queues
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        IQueryable<Notification> GetNotificationQueues(NotificationEnums.NotificationModule? module = null);

        /// <summary>
        /// Update notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateNotification(Notification model);

        /// <summary>
        /// Deactive notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel DeactiveNotification(Notification model);

        #endregion

        #region Setup

        #region Initialize Notification

        /// <summary>
        /// Get notification initialize model
        /// </summary>
        /// <param name="module"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        NotificationInitializeModel GetNotificationInitializeModel(NotificationEnums.NotificationModule module,
            string parameters);

        /// <summary>
        /// Save notification initialize
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveNotificationInitialize(NotificationInitializeModel model);

        #endregion

        #region Contact Group

        /// <summary>
        /// Get contact group setup model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ContactGroupSetupModel GetContactGroupSetupModel(int id);

        /// <summary>
        /// Save contact group setup step
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        ContactStatisticModel SaveContactGroup(ContactGroupSetupModel model, out string message);

        /// <summary>
        /// Change contact group
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="contactGroupId"></param>
        /// <returns></returns>
        ResponseModel ChangeContactGroup(int notificationId, int contactGroupId);

        /// <summary>
        /// Count contacts
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel CountContacts(ContactSearchModel model);

        /// <summary>
        /// Count contacts in a notification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int CountContacts(int id);

        /// <summary>
        /// Get contact search details models from notification id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<ContactSearchDetailsModel> GetContactSearchDetailsModels(int id);

        #endregion

        #region Notification Template 

        /// <summary>
        /// Get notification template setup model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        NotificationTemplateSetupModel GetNotificationTemplateSetupModel(int id);

        /// <summary>
        /// Save notification template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveNotificationTemplate(NotificationTemplateSetupModel model);

        #endregion

        #region Notification Configuration

        /// <summary>
        /// Get notification configuration setup model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        NotificationConfigurationSetupModel GetNotificationConfigurationSetupModel(int id);

        /// <summary>
        /// Get notification confirmation model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        NotificationConfirmationModel GetNotificationConfirmationModel(int id);

        /// <summary>
        /// Save notification configuration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveNotificationConfiguration(NotificationConfigurationSetupModel model);

        #endregion

        #endregion
    }
}