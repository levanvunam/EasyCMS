using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.ContactGroups;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Models.Notifications;
using EzCMS.Core.Models.Notifications.Setup;
using EzCMS.Core.Models.Notifications.Setup.NotificationSetup;
using EzCMS.Core.Models.NotificationTemplates;
using EzCMS.Core.Services.ContactGroups;
using EzCMS.Core.Services.Contacts;
using EzCMS.Core.Services.NotificationTemplates;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EzCMS.Core.Services.Notifications
{
    public class NotificationService : ServiceHelper, INotificationService
    {
        private readonly IContactGroupService _contactGroupService;
        private readonly IContactService _contactService;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly INotificationTemplateService _notificationTemplateService;

        public NotificationService(IRepository<Notification> notificationRepository, IContactService contactService,
            IContactGroupService contactGroupService, INotificationTemplateService notificationTemplateService)
        {
            _notificationRepository = notificationRepository;
            _contactGroupService = contactGroupService;
            _notificationTemplateService = notificationTemplateService;
            _contactService = contactService;
        }

        #region Validation

        /// <summary>
        /// Check if contact group in notification is null or empty string
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel IsNotificationSetupFilter(int id)
        {
            var notification = GetById(id);
            if (notification == null || string.IsNullOrEmpty(notification.ContactQueries))
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Notification_Message_EmptyContactQueries")
                };
            }

            return new ResponseModel
            {
                Success = true
            };
        }

        #endregion

        #region Base

        public IQueryable<Notification> GetAll()
        {
            return _notificationRepository.GetAll();
        }

        public IQueryable<Notification> Fetch(Expression<Func<Notification, bool>> expression)
        {
            return _notificationRepository.Fetch(expression);
        }

        public Notification FetchFirst(Expression<Func<Notification, bool>> expression)
        {
            return _notificationRepository.FetchFirst(expression);
        }

        public Notification GetById(object id)
        {
            return _notificationRepository.GetById(id);
        }

        internal ResponseModel Insert(Notification notification)
        {
            return _notificationRepository.Insert(notification);
        }

        internal ResponseModel Update(Notification notification)
        {
            return _notificationRepository.Update(notification);
        }

        internal ResponseModel Delete(Notification notification)
        {
            return _notificationRepository.Delete(notification);
        }

        internal ResponseModel Delete(object id)
        {
            return _notificationRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _notificationRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the notifications
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchNotifications(JqSearchIn si)
        {
            var data = GetAll();

            var notifications = Maps(data);

            return si.Search(notifications);
        }

        /// <summary>
        /// Export notifications
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var notifications = Maps(data);

            var exportData = si.Export(notifications, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Search the notified contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchNotifiedContacts(JqSearchIn si, int notificationId)
        {
            var notifiedContacts = SearchNotifiedContacts(notificationId);

            return si.Search(notifiedContacts);
        }

        /// <summary>
        /// Export notified contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public HSSFWorkbook ExportsNotifiedContacts(JqSearchIn si, GridExportMode gridExportMode, int notificationId)
        {
            var notifiedContacts = SearchNotifiedContacts(notificationId);

            var exportData = si.Export(notifiedContacts, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search notified contacts
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        private IQueryable<NotifiedContactModel> SearchNotifiedContacts(int notificationId)
        {
            var notification = FetchFirst(n => n.Id == notificationId);

            return
                SerializeUtilities.Deserialize<List<NotifiedContactModel>>(notification.NotifiedContacts).AsQueryable();
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="notifications"></param>
        /// <returns></returns>
        private IQueryable<NotificationModel> Maps(IQueryable<Notification> notifications)
        {
            return notifications.Select(notification => new NotificationModel
            {
                Id = notification.Id,
                Parameters = notification.Parameters,
                ContactQueries = notification.ContactQueries,
                NotifiedContacts = notification.NotifiedContacts,
                Module = notification.Module,
                NotificationSubject = notification.NotificationSubject,
                NotificationBody = notification.NotificationBody,
                SendTime = notification.SendTime,
                Active = notification.Active,
                RecordOrder = notification.RecordOrder,
                Created = notification.Created,
                CreatedBy = notification.CreatedBy,
                LastUpdate = notification.LastUpdate,
                LastUpdateBy = notification.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get notification detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NotificationDetailModel GetNotificationDetailModel(int? id = null)
        {
            var notification = GetById(id);
            return notification != null ? new NotificationDetailModel(notification) : null;
        }

        /// <summary>
        /// Get all notifications of module that have passed the send time
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public IQueryable<Notification> GetNotifications(NotificationEnums.NotificationModule? module = null)
        {
            if (module.HasValue)
            {
                return Fetch(notification => notification.Module == module);
            }

            return GetAll();
        }

        /// <summary>
        /// Get active notifications of module that have passed the send time
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public IQueryable<Notification> GetNotificationQueues(NotificationEnums.NotificationModule? module = null)
        {
            return GetNotifications(module).Where(notification => notification.Active);
        }

        /// <summary>
        /// Update notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateNotification(Notification model)
        {
            var notification = GetById(model.Id);
            if (notification != null)
            {
                var response = Update(model);
                return response.SetMessage(response.Success
                    ? T("Notification_Message_UpdateSuccessfully")
                    : T("Notification_Message_UpdateFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("Notification_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Deactive notification
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public ResponseModel DeactiveNotification(Notification notification)
        {
            notification.Active = false;
            return Update(notification);
        }

        #endregion

        #region Setup

        #region Initialize Notification

        /// <summary>
        /// Get notification initialize model
        /// </summary>
        /// <param name="module"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public NotificationInitializeModel GetNotificationInitializeModel(NotificationEnums.NotificationModule module,
            string parameters)
        {
            return new NotificationInitializeModel(module, parameters);
        }

        /// <summary>
        /// Save notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveNotificationInitialize(NotificationInitializeModel model)
        {
            Mapper.CreateMap<NotificationInitializeModel, Notification>();
            var notification = Mapper.Map<NotificationInitializeModel, Notification>(model);
            notification.Active = false;

            var response = Insert(notification);
            return response.SetMessage(response.Success
                ? T("Notification_Message_CreateSuccessfully")
                : T("Notification_Message_CreateFailure"));
        }

        #endregion

        #region Contact Group

        /// <summary>
        /// Get contact group setup model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ContactGroupSetupModel GetContactGroupSetupModel(int id)
        {
            var notification = GetById(id);

            if (notification != null)
            {
                return new ContactGroupSetupModel(notification);
            }

            return null;
        }

        /// <summary>
        /// Save contact group to notification
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public ContactStatisticModel SaveContactGroup(ContactGroupSetupModel model, out string message)
        {
            var notification = GetById(model.Id);
            if (notification != null)
            {
                // Deserialize contact queries string in notification model to object
                var contactSearchModels = string.IsNullOrEmpty(notification.ContactQueries)
                    ? new List<ContactSearchModel>()
                    : SerializeUtilities.Deserialize<List<ContactSearchModel>>(notification.ContactQueries);

                // Number of contacts before new queries is added
                var totalExistingContacts = _contactService.SearchContacts(contactSearchModels).Count();

                // Add new queries
                contactSearchModels.Add(model.ContactSearchModel);

                // Number of contact after new queries is added
                var totalContactsAfterAdded = _contactService.SearchContacts(contactSearchModels).Count();

                // Number of contact in new queries
                var totalNewContactsExpected = _contactService.SearchContacts(model.ContactSearchModel).Count();

                // Serialize contact queries and assign to notification model
                notification.ContactQueries = SerializeUtilities.Serialize(contactSearchModels);

                // Save new notification
                var response = Update(notification);
                if (response.Success)
                {
                    message = string.Format("{0} duplicated contact(s). {1} contact(s) added successfully.",
                        totalNewContactsExpected + totalExistingContacts - totalContactsAfterAdded,
                        totalContactsAfterAdded - totalExistingContacts);

                    return new ContactStatisticModel
                    {
                        ExistedContacts = totalNewContactsExpected + totalExistingContacts - totalContactsAfterAdded,
                        NewContacts = totalContactsAfterAdded - totalExistingContacts,
                        TotalContacts = totalContactsAfterAdded,
                        ContactSearchDetailsModel = new ContactSearchDetailsModel(model.ContactSearchModel)
                    };
                }

                message = T("Notification_Message_SavingContactGroupFailure");
                return null;
            }
            message = T("Notification_Message_ObjectNotFound");
            return null;
        }

        /// <summary>
        /// Change notification's contact group by contact group id
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="contactGroupId"></param>
        /// <returns></returns>
        public ResponseModel ChangeContactGroup(int notificationId, int contactGroupId)
        {
            // Get notification manage model by id, if it is not exist, return error
            var notification = GetById(notificationId);
            if (notification == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Notification_Message_ObjectNotFound")
                };
            }

            // Get contact group manage model by id, if it is not exist, return error
            var contactGroup = _contactGroupService.GetById(contactGroupId);
            if (contactGroup == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("ContactGroup_Message_ObjectNotFound")
                };
            }

            // Assign new contact group to notification model
            notification.ContactQueries = contactGroup.Queries;

            // Save new notification
            var response = Update(notification);
            if (response.Success)
            {
                response.Data = _contactGroupService.GetContacts(notification.ContactQueries).Count();
            }

            return response;
        }

        /// <summary>
        /// Count contacts
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel CountContacts(ContactSearchModel model)
        {
            var notification = GetById(model.NotificationId);
            if (notification != null)
            {
                // Deserialize contact queries string in notification model to object
                var contactSearchModels = string.IsNullOrEmpty(notification.ContactQueries)
                    ? new List<ContactSearchModel>()
                    : SerializeUtilities.Deserialize<List<ContactSearchModel>>(notification.ContactQueries);

                // Number of contacts before new queries is added
                var totalExistingContacts = _contactService.SearchContacts(contactSearchModels).Count();

                // Add new queries
                contactSearchModels.Add(model);

                // Number of contact after new queries is added
                var totalContactsAfterAdded = _contactService.SearchContacts(contactSearchModels).Count();

                // Number of contact in new queries
                var totalNewContactsExpected = _contactService.SearchContacts(model).Count();

                // Serialize contact queries and assign to notification model
                notification.ContactQueries = SerializeUtilities.Serialize(contactSearchModels);

                var contactStatistic = new ContactStatisticModel
                {
                    ExistedContacts = totalNewContactsExpected + totalExistingContacts - totalContactsAfterAdded,
                    NewContacts = totalContactsAfterAdded - totalExistingContacts,
                    TotalContacts = totalContactsAfterAdded
                };

                var response = new ResponseModel
                {
                    Success = true,
                    Data = contactStatistic
                };

                return response;
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("Notification_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Count contacts in a notification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int CountContacts(int id)
        {
            var notification = GetById(id);
            if (notification != null)
            {
                // Deserialize contact queries string in notification model to object
                var contactSearchModels = string.IsNullOrEmpty(notification.ContactQueries)
                    ? new List<ContactSearchModel>()
                    : SerializeUtilities.Deserialize<List<ContactSearchModel>>(notification.ContactQueries);

                // Number of contacts in notified contact queries
                return _contactService.SearchContacts(contactSearchModels).Count();
            }

            return 0;
        }

        /// <summary>
        /// Get contact search details models from notification id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<ContactSearchDetailsModel> GetContactSearchDetailsModels(int id)
        {
            var notification = GetById(id);
            if (notification != null)
            {
                return _contactService.GetContactSearchDetailsModels(notification.ContactQueries);
            }

            return new List<ContactSearchDetailsModel>();
        }

        #endregion

        #region Notification Template 

        /// <summary>
        /// Get notification template setup model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NotificationTemplateSetupModel GetNotificationTemplateSetupModel(int id)
        {
            var notification = GetById(id);
            if (notification != null)
            {
                return new NotificationTemplateSetupModel(notification);
            }

            return null;
        }

        /// <summary>
        /// Save notification template to notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveNotificationTemplate(NotificationTemplateSetupModel model)
        {
            var notification = GetById(model.Id);
            if (notification != null)
            {
                notification.NotificationSubject = model.NotificationSubject;
                notification.NotificationBody = model.NotificationBody;

                var response = Update(notification);

                return response.Success
                    ? response.SetMessage(T("Notification_Message_SavingNotificationTemplateSuccessfully"))
                    : response.SetMessage(T("Notification_Message_SavingNotificationTemplateFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("Notification_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Notification Confirmation

        /// <summary>
        /// Get notification confirmation model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NotificationConfirmationModel GetNotificationConfirmationModel(int id)
        {
            var notification = GetById(id);

            if (notification != null)
            {
                return new NotificationConfirmationModel(notification);
            }

            return null;
        }

        /// <summary>
        /// Get notification configuration setup model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NotificationConfigurationSetupModel GetNotificationConfigurationSetupModel(int id)
        {
            var notification = GetById(id);
            if (notification != null)
            {
                return new NotificationConfigurationSetupModel(notification);
            }

            return null;
        }

        /// <summary>
        /// Save contact group, notification template and notification send time
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveNotificationConfiguration(NotificationConfigurationSetupModel model)
        {
            var notification = GetById(model.Id);
            if (notification != null)
            {
                // Save contact group
                if (!string.IsNullOrEmpty(model.ContactGroupName))
                {
                    var contactGroup = new ContactGroupManageModel
                    {
                        Name = model.ContactGroupName,
                        Queries = notification.ContactQueries
                    };

                    _contactGroupService.SaveContactGroup(contactGroup);
                }

                // Save notification template
                if (!string.IsNullOrEmpty(model.NotificationTemplateName))
                {
                    var notificationTemplateManageModel = new NotificationTemplateManageModel
                    {
                        Name = model.ContactGroupName,
                        Body = notification.NotificationBody,
                        Subject = notification.NotificationSubject,
                        Module = notification.Module,
                        IsDefaultTemplate = false
                    };

                    _notificationTemplateService.SaveNotificationTemplate(notificationTemplateManageModel);
                }

                // Save notification send time and active record
                notification.SendTime = model.SendTime.HasValue ? model.SendTime : DateTime.UtcNow;
                notification.Active = true;

                var response = Update(notification);

                return response.Success
                    ? response.SetMessage(T("Notification_Message_SavingNotificationConfigrationSuccessfully"))
                    : response.SetMessage(T("Notification_Message_SavingNotificationConfigrationFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("Notification_Message_ObjectNotFound")
            };
        }

        #endregion

        #endregion
    }
}