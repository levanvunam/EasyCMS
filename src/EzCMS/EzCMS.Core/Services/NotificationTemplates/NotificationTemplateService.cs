using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Notifications.NotificationTemplates;
using EzCMS.Core.Models.NotificationTemplates;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.NotificationTemplates
{
    public class NotificationTemplateService : ServiceHelper, INotificationTemplateService
    {
        private readonly IRepository<NotificationTemplate> _notificationTemplateRepository;

        public NotificationTemplateService(IRepository<NotificationTemplate> notificationTemplateRepository)
        {
            _notificationTemplateRepository = notificationTemplateRepository;
        }

        #region Validation

        /// <summary>
        /// Check if notification template exists
        /// </summary>
        /// <param name="id">the template id</param>
        /// <param name="name">the template name</param>
        /// <returns></returns>
        public bool IsNameExisted(int? id, string name)
        {
            return Fetch(u => u.Name.Equals(name) && u.Id != id).Any();
        }

        #endregion

        /// <summary>
        /// Get notification email model assembly qualified name by module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public string GetNotificationEmailModelAssemblyName(NotificationEnums.NotificationModule module)
        {
            switch (module)
            {
                case NotificationEnums.NotificationModule.Page:
                    return typeof (NotificationPageEmailModel).AssemblyQualifiedName;
            }

            return string.Empty;
        }

        #region Base

        public IQueryable<NotificationTemplate> GetAll()
        {
            return _notificationTemplateRepository.GetAll();
        }

        public IQueryable<NotificationTemplate> Fetch(Expression<Func<NotificationTemplate, bool>> expression)
        {
            return _notificationTemplateRepository.Fetch(expression);
        }

        public NotificationTemplate FetchFirst(Expression<Func<NotificationTemplate, bool>> expression)
        {
            return _notificationTemplateRepository.FetchFirst(expression);
        }

        public NotificationTemplate GetById(object id)
        {
            return _notificationTemplateRepository.GetById(id);
        }

        public NotificationTemplate GetByName(string name)
        {
            return _notificationTemplateRepository.FetchFirst(x => x.Name.Equals(name));
        }

        internal ResponseModel Insert(NotificationTemplate notificationTemplate)
        {
            return _notificationTemplateRepository.Insert(notificationTemplate);
        }

        internal ResponseModel Update(NotificationTemplate notificationTemplate)
        {
            return _notificationTemplateRepository.Update(notificationTemplate);
        }

        internal ResponseModel Delete(NotificationTemplate notificationTemplate)
        {
            return _notificationTemplateRepository.Delete(notificationTemplate);
        }

        internal ResponseModel Delete(object id)
        {
            return _notificationTemplateRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _notificationTemplateRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid

        /// <summary>
        /// Search the notification templates
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchNotificationTemplates(JqSearchIn si)
        {
            var notificationTemplates = SearchNotificationTemplates();

            return si.Search(notificationTemplates);
        }

        /// <summary>
        /// Export notification templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var notificationTemplates = SearchNotificationTemplates();

            var exportData = si.Export(notificationTemplates, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search  notification template
        /// </summary>
        /// <returns></returns>
        private IQueryable<NotificationTemplateModel> SearchNotificationTemplates()
        {
            return GetAll().Select(c => new NotificationTemplateModel
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                IsDefaultTemplate = c.IsDefaultTemplate,
                Module = c.Module,
                RecordOrder = c.RecordOrder,
                Created = c.Created,
                CreatedBy = c.CreatedBy,
                LastUpdate = c.LastUpdate,
                LastUpdateBy = c.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Parse Email

        /// <summary>
        /// Parse the notification
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public NotificationTemplateResponseModel ParseNotification<TModel>(int? id, TModel model)
        {
            var notificationTemlate = GetById(id);
            return ParseNotification(notificationTemlate, model);
        }

        /// <summary>
        /// Parse the notification
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public NotificationTemplateResponseModel ParseNotification<TModel>(NotificationTemplate template, TModel model)
        {
            if (template == null) return null;

            var cacheName = template.Module.GetEnumName().GetTemplateCacheName(template.Body);
            var response = new NotificationTemplateResponseModel
            {
                Subject = template.Subject,
                Body = EzRazorEngineHelper.CompileAndRun(template.Body, model, cacheName)
            };

            return response;
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get notification template detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NotificationTemplateDetailModel GetNotificationTemplateDetailModel(int? id = null)
        {
            var notificationTemplate = GetById(id);
            return notificationTemplate != null ? new NotificationTemplateDetailModel(notificationTemplate) : null;
        }

        /// <summary>
        /// Update notification template data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateNotificationTemplateData(XEditableModel model)
        {
            var item = GetById(model.Pk);
            if (item != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (NotificationTemplateManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new NotificationTemplateManageModel(item);
                    manageModel.SetProperty(model.Name, value);

                    var validationResults = manageModel.ValidateModel();

                    if (validationResults.Any())
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = validationResults.BuildValidationMessages()
                        };
                    }

                    #endregion

                    item.SetProperty(model.Name, value);

                    var response = Update(item);
                    return response.SetMessage(response.Success
                        ? T("NotificationTemplateMessage_UpdateNotificationTemplateInfoSuccessfully")
                        : T("NotificationTemplate_Message_UpdateNotificationTemplateInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("NotificationTemplate_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("NotificationTemplate_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Get notification template manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NotificationTemplateManageModel GetNotificationTemplateManageModel(int? id = null)
        {
            var notificationTemplate = GetById(id);
            return notificationTemplate != null
                ? new NotificationTemplateManageModel(notificationTemplate)
                : new NotificationTemplateManageModel();
        }

        /// <summary>
        /// Get default notification template by module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public NotificationTemplate GetDefaultNotificationTemplate(NotificationEnums.NotificationModule module)
        {
            return FetchFirst(m => m.IsDefaultTemplate && m.Module == module);
        }

        /// <summary>
        /// Get all active notification templates of a module
        /// </summary>
        /// <param name="module"></param>
        /// <param name="selectDefault"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetNotificationTemplates(NotificationEnums.NotificationModule module,
            bool selectDefault = false)
        {
            return Fetch(m => m.Module == module).Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = SqlFunctions.StringConvert((double) m.Id).Trim(),
                Selected = selectDefault && m.IsDefaultTemplate
            });
        }

        /// <summary>
        /// Save notification template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveNotificationTemplate(NotificationTemplateManageModel model)
        {
            ResponseModel response;
            var notificationTemplate = GetById(model.Id);
            if (notificationTemplate != null)
            {
                notificationTemplate.Name = model.Name;
                notificationTemplate.Subject = model.Subject;
                notificationTemplate.Body = model.Body;
                notificationTemplate.Module = model.Module;
                response = Update(notificationTemplate);

                return response.SetMessage(response.Success
                    ? T("NotificationTemplate_Message_UpdateSuccessfully")
                    : T("NotificationTemplate_Message_UpdateFailure"));
            }
            Mapper.CreateMap<NotificationTemplateManageModel, NotificationTemplate>();
            notificationTemplate = Mapper.Map<NotificationTemplateManageModel, NotificationTemplate>(model);

            response = Insert(notificationTemplate);
            return response.SetMessage(response.Success
                ? T("NotificationTemplate_Message_CreateSuccessfully")
                : T("NotificationTemplate_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete the notification template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteNotificationTemplate(int id)
        {
            var notificationTemplate = GetById(id);

            if (notificationTemplate != null)
            {
                if (notificationTemplate.IsDefaultTemplate)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("NotificationTemplate_Message_CanNotDeleteDefaultTemplate")
                    };
                }

                var response = _notificationTemplateRepository.SetRecordDeleted(id);

                return response.SetMessage(response.Success
                    ? T("NotificationTemplate_Message_DeleteSuccessfully")
                    : T("NotificationTemplate_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("NotificationTemplate_Message_DeleteSuccessfully")
            };
        }

        #endregion
    }
}