using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.SubscriptionLogs;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.SubscriptionLogs
{
    public class SubscriptionLogService : ServiceHelper, ISubscriptionLogService
    {
        private readonly IRepository<SubscriptionLog> _subscriptionLogRepository;

        public SubscriptionLogService(IRepository<SubscriptionLog> subscriptionLogRepository)
        {
            _subscriptionLogRepository = subscriptionLogRepository;
        }

        #region Manage

        /// <summary>
        /// Save Subscription Log
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveSubscriptionLog(SubscriptionLogManageModel model)
        {
            if (model.NotifySubscribers && !string.IsNullOrEmpty(model.ChangeLog))
            {
                Mapper.CreateMap<SubscriptionLogManageModel, SubscriptionLog>();
                var subscriptionLog = Mapper.Map<SubscriptionLogManageModel, SubscriptionLog>(model);
                var response = Insert(subscriptionLog);
                return response.SetMessage(response.Success
                    ? T("SubscriptionLog_Message_CreateSuccessfully")
                    : T("SubscriptionLog_Message_CreateFailure"));
            }

            return new ResponseModel
            {
                Success = true
            };
        }

        #endregion

        /// <summary>
        /// Deactive Subscription Log
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel DeactiveSubscriptionLog(SubscriptionLog model)
        {
            model.Active = false;
            return Update(model);
        }

        public ResponseModel DisabledSubscriptionNighly(SubscriptionLog model)
        {
            model.IsNightlySent = true;
            return Update(model);
        }

        /// <summary>
        /// Disabled Instantly Subscription after sent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel DisabledSubscriptionDirectly(SubscriptionLog model)
        {
            model.IsDirectlySent = true;
            return Update(model);
        }

        /// <summary>
        /// Delete subdcription logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public ResponseModel DeleteSubscriptionLogs(DateTime date)
        {
            var logs = Fetch(l => l.Created < date);
            _subscriptionLogRepository.Delete(logs);
            return new ResponseModel
            {
                Success = true
            };
        }

        /// <summary>
        /// Get all logs of module from start time
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="module"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IQueryable<SubscriptionLog> GetLogs(
            SubscriptionEnums.SubscriptionType? type = null,
            DateTime? startTime = null, SubscriptionEnums.SubscriptionModule? module = null)
        {
            //Filter by start time
            var logs = Fetch(l => !startTime.HasValue || l.Created >= startTime);

            // Filter by module
            if (module.HasValue)
            {
                logs = logs.Where(l => l.Module == module);
            }

            //Filter by type
            if (type != null)
            {
                switch (type)
                {
                    case SubscriptionEnums.SubscriptionType.Instantly:
                        logs = logs.Where(l => !l.IsDirectlySent);
                        break;
                    case SubscriptionEnums.SubscriptionType.Midnight:
                        logs = logs.Where(l => !l.IsNightlySent);
                        break;
                }
            }

            return logs;
        }

        #region Base

        public IQueryable<SubscriptionLog> GetAll()
        {
            return _subscriptionLogRepository.GetAll();
        }

        public IQueryable<SubscriptionLog> Fetch(Expression<Func<SubscriptionLog, bool>> expression)
        {
            return _subscriptionLogRepository.Fetch(expression);
        }

        public SubscriptionLog FetchFirst(Expression<Func<SubscriptionLog, bool>> expression)
        {
            return _subscriptionLogRepository.FetchFirst(expression);
        }

        public SubscriptionLog GetById(object id)
        {
            return _subscriptionLogRepository.GetById(id);
        }

        internal ResponseModel Insert(SubscriptionLog subscriptionLog)
        {
            return _subscriptionLogRepository.Insert(subscriptionLog);
        }

        internal ResponseModel Update(SubscriptionLog subscriptionLog)
        {
            return _subscriptionLogRepository.Update(subscriptionLog);
        }

        internal ResponseModel Delete(SubscriptionLog subscriptionLog)
        {
            return _subscriptionLogRepository.Delete(subscriptionLog);
        }

        internal ResponseModel Delete(object id)
        {
            return _subscriptionLogRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _subscriptionLogRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the subscription logs.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchSubscriptionLogs(JqSearchIn si)
        {
            var data = GetAll();

            var subscriptionLogs = Maps(data);

            return si.Search(subscriptionLogs);
        }

        /// <summary>
        /// Export subscription logs
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var subscriptionLogs = Maps(data);

            var exportData = si.Export(subscriptionLogs, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private methods

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="subscriptionLogs"></param>
        /// <returns></returns>
        private IQueryable<SubscriptionLogModel> Maps(IQueryable<SubscriptionLog> subscriptionLogs)
        {
            return subscriptionLogs.Select(m => new SubscriptionLogModel
            {
                Id = m.Id,
                Module = m.Module,
                Parameters = m.Parameters,
                ChangeLog = m.ChangeLog,
                Active = m.Active,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion

        #endregion
    }
}