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
using EzCMS.Core.Models.StyleLogs;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Services.StyleLogs
{
    public class StyleLogService : ServiceHelper, IStyleLogService
    {
        private readonly IRepository<StyleLog> _styleLogRepository;
        private readonly IRepository<Style> _styleRepository;

        public StyleLogService(IRepository<StyleLog> styleLogRepository, IRepository<Style> styleRepository)
        {
            _styleLogRepository = styleLogRepository;
            _styleRepository = styleRepository;
        }

        #region Base

        public IQueryable<StyleLog> GetAll()
        {
            return _styleLogRepository.GetAll();
        }

        public IQueryable<StyleLog> Fetch(Expression<Func<StyleLog, bool>> expression)
        {
            return _styleLogRepository.Fetch(expression);
        }

        public StyleLog FetchFirst(Expression<Func<StyleLog, bool>> expression)
        {
            return _styleLogRepository.FetchFirst(expression);
        }

        public StyleLog GetById(object id)
        {
            return _styleLogRepository.GetById(id);
        }

        internal ResponseModel Insert(StyleLog styleLog)
        {
            return _styleLogRepository.Insert(styleLog);
        }

        internal ResponseModel Update(StyleLog styleLog)
        {
            return _styleLogRepository.Update(styleLog);
        }

        internal ResponseModel Delete(StyleLog styleLog)
        {
            return _styleLogRepository.Delete(styleLog);
        }

        internal ResponseModel Delete(object id)
        {
            return _styleLogRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _styleLogRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Logs

        /// <summary>
        /// Save current page to audit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveStyleLog(StyleLogManageModel model)
        {
            var style = _styleRepository.GetById(model.StyleId);
            if (style != null)
            {
                /*
                 * Map Style log model to log entity
                 * Get last updated version of Style log
                 * If there are nothing change then do not do anything
                 * Otherwise insert log
                 */
                Mapper.CreateMap<StyleLogManageModel, StyleLog>();
                var log = Mapper.Map<StyleLogManageModel, StyleLog>(model);

                var styleLog = GetAll().Where(a => a.StyleId == style.Id).OrderByDescending(a => a.Id).FirstOrDefault();

                log.ChangeLog = styleLog != null
                    ? ChangeLog(styleLog, model)
                    : "** Create Style **";

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
                Message = T("Style_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete style logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public ResponseModel DeleteStyleLogs(DateTime date)
        {
            var logs = Fetch(l => l.Created < date);
            _styleLogRepository.Delete(logs);
            return new ResponseModel
            {
                Success = true
            };
        }

        /// <summary>
        /// Update data and create change log
        /// </summary>
        /// <param name="styleLog"></param>
        /// <param name="styleLogModel"></param>
        /// <returns></returns>
        private string ChangeLog(StyleLog styleLog, StyleLogManageModel styleLogModel)
        {
            var changeLog = new StringBuilder();
            const string format = "- Update field: {0}\n";
            if (!ConvertUtilities.Compare(styleLog.Name, styleLogModel.Name))
            {
                changeLog.AppendFormat(format, "Name");
                styleLog.Name = styleLogModel.Name;
            }
            if (!ConvertUtilities.Compare(styleLog.Content, styleLogModel.Content))
            {
                changeLog.AppendFormat(format, "Content");
                styleLog.Content = styleLogModel.Content;
            }

            if (!string.IsNullOrEmpty(changeLog.ToString()))
            {
                changeLog.Insert(0, "** Update Style **\n");
            }

            return changeLog.ToString();
        }

        #endregion
    }
}