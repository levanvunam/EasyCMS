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
using EzCMS.Core.Models.PageTemplateLogs;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Services.PageTemplateLogs
{
    public class PageTemplateLogService : ServiceHelper, IPageTemplateLogService
    {
        private readonly IRepository<PageTemplateLog> _pageTemplateLogRepository;
        private readonly IHierarchyRepository<PageTemplate> _pageTemplateRepository;

        public PageTemplateLogService(IRepository<PageTemplateLog> pageTemplateLogRepository,
            IHierarchyRepository<PageTemplate> pageTemplateRepository)
        {
            _pageTemplateLogRepository = pageTemplateLogRepository;
            _pageTemplateRepository = pageTemplateRepository;
        }

        #region Base

        public IQueryable<PageTemplateLog> GetAll()
        {
            return _pageTemplateLogRepository.GetAll();
        }

        public IQueryable<PageTemplateLog> Fetch(Expression<Func<PageTemplateLog, bool>> expression)
        {
            return _pageTemplateLogRepository.Fetch(expression);
        }

        public PageTemplateLog FetchFirst(Expression<Func<PageTemplateLog, bool>> expression)
        {
            return _pageTemplateLogRepository.FetchFirst(expression);
        }

        public PageTemplateLog GetById(object id)
        {
            return _pageTemplateLogRepository.GetById(id);
        }

        internal ResponseModel Insert(PageTemplateLog pageTemplateLog)
        {
            return _pageTemplateLogRepository.Insert(pageTemplateLog);
        }

        internal ResponseModel Update(PageTemplateLog pageTemplateLog)
        {
            return _pageTemplateLogRepository.Update(pageTemplateLog);
        }

        internal ResponseModel Delete(PageTemplateLog pageTemplateLog)
        {
            return _pageTemplateLogRepository.Delete(pageTemplateLog);
        }

        internal ResponseModel Delete(object id)
        {
            return _pageTemplateLogRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _pageTemplateLogRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Logs

        /// <summary>
        /// Save current page to audit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SavePageTemplateLog(PageTemplateLogManageModel model)
        {
            var pageTemplate = _pageTemplateRepository.GetById(model.PageTemplateId);
            if (pageTemplate != null)
            {
                /*
                 * Map page template log model to log entity
                 * Get last updated version of template log
                 * If there are nothing change then do not do anything
                 * Otherwise insert log
                 */
                Mapper.CreateMap<PageTemplateLogManageModel, PageTemplateLog>();
                var log = Mapper.Map<PageTemplateLogManageModel, PageTemplateLog>(model);

                var pageTemplateLog =
                    GetAll()
                        .Where(a => a.PageTemplateId == pageTemplate.Id)
                        .OrderByDescending(a => a.Id)
                        .FirstOrDefault();

                log.ChangeLog = pageTemplateLog != null
                    ? ChangeLog(pageTemplateLog, model)
                    : "** Create Page Template **";

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
                Message = T("Page_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete page template logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public ResponseModel DeletePageTemplateLogs(DateTime date)
        {
            var pageTemplateLogs = Fetch(log => log.Created < date);
            _pageTemplateLogRepository.Delete(pageTemplateLogs);

            return new ResponseModel
            {
                Success = true
            };
        }

        /// <summary>
        /// Update data and create change log
        /// </summary>
        /// <param name="pageTemplateLog"></param>
        /// <param name="pageTemplateLogModel"></param>
        /// <returns></returns>
        private string ChangeLog(PageTemplateLog pageTemplateLog, PageTemplateLogManageModel pageTemplateLogModel)
        {
            var changeLog = new StringBuilder();
            const string format = "- Update field: {0}\n";
            if (!ConvertUtilities.Compare(pageTemplateLog.Name, pageTemplateLogModel.Name))
            {
                changeLog.AppendFormat(format, "Name");
                pageTemplateLog.Name = pageTemplateLogModel.Name;
            }
            if (!ConvertUtilities.Compare(pageTemplateLog.Content, pageTemplateLogModel.Content))
            {
                changeLog.AppendFormat(format, "Content");
                pageTemplateLog.Content = pageTemplateLogModel.Content;
            }
            if (!ConvertUtilities.Compare(pageTemplateLog.ParentId, pageTemplateLogModel.ParentId))
            {
                changeLog.AppendFormat(format, "ParentId");
                pageTemplateLog.ParentId = pageTemplateLogModel.ParentId;
            }

            if (!string.IsNullOrEmpty(changeLog.ToString()))
            {
                changeLog.Insert(0, "** Update Page Template **\n");
            }

            return changeLog.ToString();
        }

        #endregion
    }
}