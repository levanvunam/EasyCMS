using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using EzCMS.Core.Models.WidgetTemplateLogs;
using EzCMS.Entity.Entities.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace EzCMS.Core.Services.WidgetTemplateLogs
{
    public class WidgetTemplateLogService : ServiceHelper, IWidgetTemplateLogService
    {
        private readonly IRepository<WidgetTemplateLog> _templateLogRepository;
        private readonly IRepository<WidgetTemplate> _templateRepository;

        public WidgetTemplateLogService(IRepository<WidgetTemplateLog> templateLogRepository,
            IRepository<WidgetTemplate> templateRepository)
        {
            _templateLogRepository = templateLogRepository;
            _templateRepository = templateRepository;
        }

        #region Base

        public IQueryable<WidgetTemplateLog> GetAll()
        {
            return _templateLogRepository.GetAll();
        }

        public IQueryable<WidgetTemplateLog> Fetch(Expression<Func<WidgetTemplateLog, bool>> expression)
        {
            return _templateLogRepository.Fetch(expression);
        }

        public WidgetTemplateLog FetchFirst(Expression<Func<WidgetTemplateLog, bool>> expression)
        {
            return _templateLogRepository.FetchFirst(expression);
        }

        public WidgetTemplateLog GetById(object id)
        {
            return _templateLogRepository.GetById(id);
        }

        internal ResponseModel Insert(WidgetTemplateLog widgetTemplateLog)
        {
            return _templateLogRepository.Insert(widgetTemplateLog);
        }

        internal ResponseModel Update(WidgetTemplateLog widgetTemplateLog)
        {
            return _templateLogRepository.Update(widgetTemplateLog);
        }

        internal ResponseModel Delete(WidgetTemplateLog widgetTemplateLog)
        {
            return _templateLogRepository.Delete(widgetTemplateLog);
        }

        internal ResponseModel Delete(object id)
        {
            return _templateLogRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _templateLogRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Logs

        /// <summary>
        /// Save current page to audit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveTemplateLog(WidgetTemplateLogManageModel model)
        {
            var template = _templateRepository.GetById(model.TemplateId);
            if (template != null)
            {
                /*
                 * Map template log model to log entity
                 * Get last updated version of template log
                 * If there are nothing change then do not do anything
                 * Otherwise insert log
                 */
                Mapper.CreateMap<WidgetTemplateLogManageModel, WidgetTemplateLog>();
                var log = Mapper.Map<WidgetTemplateLogManageModel, WidgetTemplateLog>(model);

                var templateLog =
                    GetAll().Where(a => a.TemplateId == template.Id).OrderByDescending(a => a.Id).FirstOrDefault();

                log.ChangeLog = templateLog != null
                    ? ChangeLog(templateLog, model)
                    : "** Create Template **";

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
                Message = T("WidgetTemplate_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete template logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public ResponseModel DeleteTemplateLogs(DateTime date)
        {
            var logs = Fetch(l => l.Created < date);
            _templateLogRepository.Delete(logs);

            return new ResponseModel
            {
                Success = true
            };
        }

        /// <summary>
        /// Update data and create change log
        /// </summary>
        /// <param name="widgetTemplateLog"></param>
        /// <param name="widgetTemplateLogModel"></param>
        /// <returns></returns>
        private string ChangeLog(WidgetTemplateLog widgetTemplateLog, WidgetTemplateLogManageModel widgetTemplateLogModel)
        {
            var changeLog = new StringBuilder();
            const string format = "- Update field: {0}\n";
            if (!ConvertUtilities.Compare(widgetTemplateLog.Name, widgetTemplateLogModel.Name))
            {
                changeLog.AppendFormat(format, "Name");
                widgetTemplateLog.Name = widgetTemplateLogModel.Name;
            }
            if (!ConvertUtilities.Compare(widgetTemplateLog.Content, widgetTemplateLogModel.Content))
            {
                changeLog.AppendFormat(format, "Content");
                widgetTemplateLog.Content = widgetTemplateLogModel.Content;
            }

            if (!string.IsNullOrEmpty(changeLog.ToString()))
            {
                changeLog.Insert(0, "** Update Template **\n");
            }

            return changeLog.ToString();
        }

        #endregion
    }
}