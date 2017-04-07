using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using EzCMS.Core.Models.ScriptLogs;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Services.ScriptLogs
{
    public class ScriptLogService : ServiceHelper, IScriptLogService
    {
        private readonly IRepository<ScriptLog> _scriptLogRepository;
        private readonly IRepository<Script> _scriptRepository;

        public ScriptLogService(IRepository<ScriptLog> scriptLogRepository, IRepository<Script> scriptRepository)
        {
            _scriptLogRepository = scriptLogRepository;
            _scriptRepository = scriptRepository;
        }

        #region Base

        public IQueryable<ScriptLog> GetAll()
        {
            return _scriptLogRepository.GetAll();
        }

        public IQueryable<ScriptLog> Fetch(Expression<Func<ScriptLog, bool>> expression)
        {
            return _scriptLogRepository.Fetch(expression);
        }

        public ScriptLog FetchFirst(Expression<Func<ScriptLog, bool>> expression)
        {
            return _scriptLogRepository.FetchFirst(expression);
        }

        public ScriptLog GetById(object id)
        {
            return _scriptLogRepository.GetById(id);
        }

        internal ResponseModel Insert(ScriptLog scriptLog)
        {
            return _scriptLogRepository.Insert(scriptLog);
        }

        internal ResponseModel Update(ScriptLog scriptLog)
        {
            return _scriptLogRepository.Update(scriptLog);
        }

        internal ResponseModel Delete(ScriptLog scriptLog)
        {
            return _scriptLogRepository.Delete(scriptLog);
        }

        internal ResponseModel Delete(object id)
        {
            return _scriptLogRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _scriptLogRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Logs

        /// <summary>
        /// Save current page to audit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveScriptLog(ScriptLogManageModel model)
        {
            var script = _scriptRepository.GetById(model.ScriptId);
            if (script != null)
            {
                /*
                 * Map Script log model to log entity
                 * Get last updated version of Script log
                 * If there are nothing change then do not do anything
                 * Otherwise insert log
                 */
                Mapper.CreateMap<ScriptLogManageModel, ScriptLog>();
                var log = Mapper.Map<ScriptLogManageModel, ScriptLog>(model);

                var scriptLog =
                    GetAll().Where(a => a.ScriptId == script.Id).OrderByDescending(a => a.Id).FirstOrDefault();

                log.ChangeLog = scriptLog != null
                    ? ChangeLog(scriptLog, model)
                    : "** Create Script **";

                if (string.IsNullOrEmpty(log.ChangeLog))
                {
                    return new ResponseModel
                    {
                        Success = true
                    };
                }
                log.SessionId = HttpContext.Current.Session.SessionID;
                return Insert(log);
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("Script_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete script logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public ResponseModel DeleteScriptLogs(DateTime date)
        {
            var logs = Fetch(l => l.Created < date);
            _scriptLogRepository.Delete(logs);
            return new ResponseModel
            {
                Success = true
            };
        }

        /// <summary>
        /// Update data and create change log
        /// </summary>
        /// <param name="scriptLog"></param>
        /// <param name="scriptLogModel"></param>
        /// <returns></returns>
        private string ChangeLog(ScriptLog scriptLog, ScriptLogManageModel scriptLogModel)
        {
            var changeLog = new StringBuilder();
            const string format = "- Update field: {0}\n";
            if (!ConvertUtilities.Compare(scriptLog.Name, scriptLogModel.Name))
            {
                changeLog.AppendFormat(format, "Name");
                scriptLog.Name = scriptLogModel.Name;
            }
            if (!ConvertUtilities.Compare(scriptLog.Content, scriptLogModel.Content))
            {
                changeLog.AppendFormat(format, "Content");
                scriptLog.Content = scriptLogModel.Content;
            }

            if (!string.IsNullOrEmpty(changeLog.ToString()))
            {
                changeLog.Insert(0, "** Update Script **\n");
            }

            return changeLog.ToString();
        }

        #endregion
    }
}