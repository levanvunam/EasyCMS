using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Subscriptions.SubscriptionTemplates;
using EzCMS.Core.Models.SubscriptionTemplates;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EzCMS.Core.Services.SubscriptionTemplates
{
    public class SubscriptionTemplateService : ServiceHelper, ISubscriptionTemplateService
    {
        private readonly IRepository<SubscriptionTemplate> _subscriptionTemplateRepository;

        public SubscriptionTemplateService(IRepository<SubscriptionTemplate> subscriptionTemplateRepository)
        {
            _subscriptionTemplateRepository = subscriptionTemplateRepository;
        }

        /// <summary>
        /// Parse the Subscription
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="module"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public SubscriptionTemplateResponseModel ParseSubscription<TModel>(SubscriptionEnums.SubscriptionModule module,
            TModel model)
        {
            var subscriptionTemlate = _subscriptionTemplateRepository.FetchFirst(e => e.Module == module);
            return ParseSubscription(subscriptionTemlate, model);
        }

        /// <summary>
        /// Parse the Subscription
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public SubscriptionTemplateResponseModel ParseSubscription<TModel>(SubscriptionTemplate template, TModel model)
        {
            if (template == null) return null;

            var bodyCacheName = template.Body.GetEnumName().GetTemplateCacheName(template.Body);
            var response = new SubscriptionTemplateResponseModel
            {
                Body = EzRazorEngineHelper.CompileAndRun(template.Body, model, bodyCacheName)
            };

            return response;
        }

        /// <summary>
        /// Get notification email model assembly name
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public string GetNotificationEmailModelAssemblyName(SubscriptionEnums.SubscriptionModule module)
        {
            switch (module)
            {
                case SubscriptionEnums.SubscriptionModule.Page:
                    return typeof(List<PageSubscriptionEmailModel>).AssemblyQualifiedName;
            }

            return string.Empty;
        }

        #region Base

        public IQueryable<SubscriptionTemplate> GetAll()
        {
            return _subscriptionTemplateRepository.GetAll();
        }

        public IQueryable<SubscriptionTemplate> Fetch(Expression<Func<SubscriptionTemplate, bool>> expression)
        {
            return _subscriptionTemplateRepository.Fetch(expression);
        }

        public SubscriptionTemplate FetchFirst(Expression<Func<SubscriptionTemplate, bool>> expression)
        {
            return _subscriptionTemplateRepository.FetchFirst(expression);
        }

        public SubscriptionTemplate GetById(object id)
        {
            return _subscriptionTemplateRepository.GetById(id);
        }

        internal ResponseModel Insert(SubscriptionTemplate subscriptionTemplate)
        {
            return _subscriptionTemplateRepository.Insert(subscriptionTemplate);
        }

        internal ResponseModel Update(SubscriptionTemplate subscriptionTemplate)
        {
            return _subscriptionTemplateRepository.Update(subscriptionTemplate);
        }

        internal ResponseModel Delete(SubscriptionTemplate subscriptionTemplate)
        {
            return _subscriptionTemplateRepository.Delete(subscriptionTemplate);
        }

        internal ResponseModel Delete(object id)
        {
            return _subscriptionTemplateRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _subscriptionTemplateRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid

        /// <summary>
        /// Search the templates
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchSubscriptionTemplates(JqSearchIn si)
        {
            var data = GetAll();

            var subscriptionTemplates = Maps(data);

            return si.Search(subscriptionTemplates);
        }

        /// <summary>
        /// Export payment methods
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var subscriptionTemplates = Maps(data);

            var exportData = si.Export(subscriptionTemplates, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search subscription templates
        /// </summary>
        /// <returns></returns>
        private IQueryable<SubscriptionTemplateModel> Maps(IQueryable<SubscriptionTemplate> subscriptionTemplates)
        {
            return subscriptionTemplates.Select(c => new SubscriptionTemplateModel
            {
                Id = c.Id,
                Name = c.Name,
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

        #region Manage

        /// <summary>
        /// Get SubscriptionTemplate manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SubscriptionTemplateManageModel GetSubscriptionTemplateManageModel(int? id = null)
        {
            var subscriptionTemplate = GetById(id);
            if (subscriptionTemplate != null)
            {
                return new SubscriptionTemplateManageModel(subscriptionTemplate);
            }
            return new SubscriptionTemplateManageModel();
        }


        /// <summary>
        /// Save SubscriptionTemplate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveSubscriptionTemplate(SubscriptionTemplateManageModel model)
        {
            var subscriptionTemplate = GetById(model.Id);
            if (subscriptionTemplate != null)
            {
                subscriptionTemplate.Body = model.Body;
                var response = Update(subscriptionTemplate);
                return response.SetMessage(response.Success
                    ? T("SubscriptionTemplate_Message_UpdateSuccessfully")
                    : T("SubscriptionTemplate_Message_UpdateFailure"));
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("SubscriptionTemplate_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}