using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.ProtectedDocumentLogs;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.ProtectedDocumentLogs
{
    public class ProtectedDocumentLogService : ServiceHelper, IProtectedDocumentLogService
    {
        private readonly IRepository<ProtectedDocumentLog> _protectedDocumentLogRepository;

        public ProtectedDocumentLogService(IRepository<ProtectedDocumentLog> protectedDocumentLogRepository)
        {
            _protectedDocumentLogRepository = protectedDocumentLogRepository;
        }

        #region Manage

        /// <summary>
        /// Save ProtectedDocumentLog
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ResponseModel SaveProtectedDocumentLog(string path)
        {
            if (WorkContext.CurrentUser != null)
            {
                var protectedDocumentLog = FetchFirst(l => l.UserId == WorkContext.CurrentUser.Id && l.Path.Equals(path));
                if (protectedDocumentLog == null)
                {
                    protectedDocumentLog = new ProtectedDocumentLog
                    {
                        Path = path,
                        UserId = WorkContext.CurrentUser.Id
                    };
                    var response = Insert(protectedDocumentLog);
                    return response.SetMessage(response.Success
                        ? T("ProtectedDocumentLog_Message_CreateSuccessfully")
                        : T("ProtectedDocumentLog_Message_CreateFailure"));
                }
            }

            return new ResponseModel
            {
                Success = true
            };
        }

        #endregion

        /// <summary>
        /// Get current user logs
        /// </summary>
        /// <returns></returns>
        public IQueryable<ProtectedDocumentLogModel> GetCurrentUserLogs()
        {
            if (WorkContext.CurrentUser == null)
            {
                return new List<ProtectedDocumentLogModel>().AsQueryable();
            }
            return Maps(Fetch(l => l.Id == WorkContext.CurrentUser.Id));
        }

        /// <summary>
        /// Get current user logs
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<ProtectedDocumentLogModel> GetCurrentUserRelatedLogs(string path)
        {
            if (WorkContext.CurrentUser == null)
            {
                return new List<ProtectedDocumentLogModel>();
            }
            return Maps(Fetch(l => l.UserId == WorkContext.CurrentUser.Id && l.Path.StartsWith(path))).ToList();
        }

        /// <summary>
        /// Delete protected document logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public ResponseModel DeleteProtectedDocumentLogs(DateTime date)
        {
            var logs = Fetch(l => l.Created < date);
            _protectedDocumentLogRepository.Delete(logs);
            return new ResponseModel
            {
                Success = true
            };
        }

        #region Base

        public IQueryable<ProtectedDocumentLog> GetAll()
        {
            return _protectedDocumentLogRepository.GetAll();
        }

        public IQueryable<ProtectedDocumentLog> Fetch(Expression<Func<ProtectedDocumentLog, bool>> expression)
        {
            return _protectedDocumentLogRepository.Fetch(expression);
        }

        public ProtectedDocumentLog FetchFirst(Expression<Func<ProtectedDocumentLog, bool>> expression)
        {
            return _protectedDocumentLogRepository.FetchFirst(expression);
        }

        public ProtectedDocumentLog GetById(object id)
        {
            return _protectedDocumentLogRepository.GetById(id);
        }

        internal ResponseModel Insert(ProtectedDocumentLog protectedDocumentLog)
        {
            return _protectedDocumentLogRepository.Insert(protectedDocumentLog);
        }

        internal ResponseModel Update(ProtectedDocumentLog protectedDocumentLog)
        {
            return _protectedDocumentLogRepository.Update(protectedDocumentLog);
        }

        internal ResponseModel Delete(ProtectedDocumentLog protectedDocumentLog)
        {
            return _protectedDocumentLogRepository.Delete(protectedDocumentLog);
        }

        internal ResponseModel Delete(object id)
        {
            return _protectedDocumentLogRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _protectedDocumentLogRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the protected document logs
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchProtectedDocumentLogs(JqSearchIn si)
        {
            var data = GetAll();
            var protectedDocumentLogs = Maps(data);
            return si.Search(protectedDocumentLogs);
        }

        /// <summary>
        /// Export protected document logs
        /// </summary>
        /// <returns></returns>
        public HSSFWorkbook Exports()
        {
            var data = GetAll();
            var protectedDocumentLogs = Maps(data);
            return ExcelUtilities.CreateWorkBook(protectedDocumentLogs);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="protectedDocumentLogs"></param>
        /// <returns></returns>
        private IQueryable<ProtectedDocumentLogModel> Maps(IQueryable<ProtectedDocumentLog> protectedDocumentLogs)
        {
            return protectedDocumentLogs.Select(m => new ProtectedDocumentLogModel
            {
                Id = m.Id,
                Path = m.Path,
                UserId = m.UserId,
                Username = string.IsNullOrEmpty(m.User.Username) ? m.User.Email : m.User.Username,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion
    }
}