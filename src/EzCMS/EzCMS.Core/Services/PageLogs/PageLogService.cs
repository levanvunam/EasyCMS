using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using EzCMS.Core.Models.PageLogs;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Services.PageLogs
{
    public class PageLogService : ServiceHelper, IPageLogService
    {
        private readonly IRepository<PageLog> _pageLogRepository;
        private readonly IHierarchyRepository<Page> _pageRepository;

        public PageLogService(IRepository<PageLog> pageLogRepository, IHierarchyRepository<Page> pageRepository)
        {
            _pageLogRepository = pageLogRepository;
            _pageRepository = pageRepository;
        }

        #region Grid Search

        /// <summary>
        /// Search the page logs
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchPageLogs(JqSearchIn si)
        {
            var pageLogs = GetAll().Select(audit => new PageLogModel
            {
                Id = audit.Id,
                PageId = audit.PageId,
                PageTitle = audit.Page.Title,
                Title = audit.Title,
                ChangeLog = audit.ChangeLog,
                PageTemplateId = audit.PageTemplateId,
                FileTemplateId = audit.FileTemplateId,
                BodyTemplateId = audit.BodyTemplateId,
                Status = audit.Status,
                FriendlyUrl = audit.FriendlyUrl,
                ParentId = audit.ParentId,
                RecordOrder = audit.RecordOrder,
                Created = audit.Created,
                CreatedBy = audit.CreatedBy,
                LastUpdate = audit.LastUpdate,
                LastUpdateBy = audit.LastUpdateBy
            });

            return si.Search(pageLogs);
        }

        #endregion

        #region Base

        public IQueryable<PageLog> GetAll()
        {
            return _pageLogRepository.GetAll();
        }

        public IQueryable<PageLog> Fetch(Expression<Func<PageLog, bool>> expression)
        {
            return _pageLogRepository.Fetch(expression);
        }

        public PageLog FetchFirst(Expression<Func<PageLog, bool>> expression)
        {
            return _pageLogRepository.FetchFirst(expression);
        }

        public PageLog GetById(object id)
        {
            return _pageLogRepository.GetById(id);
        }

        internal ResponseModel Insert(PageLog pageLog)
        {
            return _pageLogRepository.Insert(pageLog);
        }

        internal ResponseModel Update(PageLog pageLog)
        {
            return _pageLogRepository.Update(pageLog);
        }

        internal ResponseModel Delete(PageLog pageLog)
        {
            return _pageLogRepository.Delete(pageLog);
        }

        internal ResponseModel Delete(object id)
        {
            return _pageLogRepository.Delete(id);
        }

        #endregion

        #region Logs

        /// <summary>
        /// Save current page to audit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SavePageLog(PageLogManageModel model)
        {
            var page = _pageRepository.GetById(model.PageId);
            if (page != null)
            {
                /*
                 * Map page log model to log entity
                 * Get last updated version of page log
                 * Create Change Log
                 * If there are nothing change then do not do anything
                 * Otherwise insert log
                 */
                Mapper.CreateMap<PageLogManageModel, PageLog>();
                var log = Mapper.Map<PageLogManageModel, PageLog>(model);

                var pageLog = GetAll().Where(a => a.PageId == page.Id).OrderByDescending(a => a.Id).FirstOrDefault();

                log.ChangeLog = pageLog != null
                    ? ChangeLog(pageLog, model)
                    : "** Create Page **";

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
        /// Delete page logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public ResponseModel DeletePageLogs(DateTime date)
        {
            var logs = Fetch(log => log.Created < date);
            _pageLogRepository.Delete(logs);

            return new ResponseModel
            {
                Success = true
            };
        }

        /// <summary>
        /// Update data and create change log
        /// </summary>
        /// <param name="pageLog"></param>
        /// <param name="pageLogModel"></param>
        /// <returns></returns>
        private string ChangeLog(PageLog pageLog, PageLogManageModel pageLogModel)
        {
            var changeLog = new StringBuilder();
            const string format = "- Update field: {0}\n";
            if (!ConvertUtilities.Compare(pageLog.Title, pageLogModel.Title))
            {
                changeLog.AppendFormat(format, "Title");
                pageLog.Title = pageLogModel.Title;
            }
            if (!ConvertUtilities.Compare(pageLog.FriendlyUrl, pageLogModel.FriendlyUrl))
            {
                changeLog.AppendFormat(format, "FriendlyUrl");
                pageLog.FriendlyUrl = pageLogModel.FriendlyUrl;
            }
            if (!ConvertUtilities.Compare(pageLog.Content, pageLogModel.Content))
            {
                changeLog.AppendFormat(format, "Content");
                pageLog.Content = pageLogModel.Content;
            }
            if (!ConvertUtilities.Compare(pageLog.ContentWorking, pageLogModel.ContentWorking))
            {
                changeLog.AppendFormat(format, "ContentWorking");
                pageLog.ContentWorking = pageLogModel.ContentWorking;
            }
            if (!ConvertUtilities.Compare(pageLog.Abstract, pageLogModel.Abstract))
            {
                changeLog.AppendFormat(format, "Abstract");
                pageLog.Abstract = pageLogModel.Abstract;
            }
            if (!ConvertUtilities.Compare(pageLog.AbstractWorking, pageLogModel.AbstractWorking))
            {
                changeLog.AppendFormat(format, "AbstractWorking");
                pageLog.AbstractWorking = pageLogModel.AbstractWorking;
            }
            if (!ConvertUtilities.Compare(pageLog.Status, pageLogModel.Status))
            {
                changeLog.AppendFormat(format, "Status");
                pageLog.Status = pageLogModel.Status;
            }
            if (!ConvertUtilities.Compare(pageLog.Keywords, pageLogModel.Keywords))
            {
                changeLog.AppendFormat(format, "Keywords");
                pageLog.Keywords = pageLogModel.Keywords;
            }
            if (!ConvertUtilities.Compare(pageLog.PageTemplateId, pageLogModel.PageTemplateId))
            {
                changeLog.AppendFormat(format, "PageTemplateId");
                pageLog.PageTemplateId = pageLogModel.PageTemplateId;
            }
            if (!ConvertUtilities.Compare(pageLog.FileTemplateId, pageLogModel.FileTemplateId))
            {
                changeLog.AppendFormat(format, "FileTemplateId");
                pageLog.FileTemplateId = pageLogModel.FileTemplateId;
            }
            if (!ConvertUtilities.Compare(pageLog.BodyTemplateId, pageLogModel.BodyTemplateId))
            {
                changeLog.AppendFormat(format, "BodyTemplateId");
                pageLog.BodyTemplateId = pageLogModel.BodyTemplateId;
            }
            if (!ConvertUtilities.Compare(pageLog.ParentId, pageLogModel.ParentId))
            {
                changeLog.AppendFormat(format, "ParentId");
                pageLog.ParentId = pageLogModel.ParentId;
            }
            if (!ConvertUtilities.Compare(pageLog.IncludeInSiteNavigation, pageLogModel.IncludeInSiteNavigation))
            {
                changeLog.AppendFormat(format, "IncludeInSiteNavigation");
                pageLog.IncludeInSiteNavigation = pageLogModel.IncludeInSiteNavigation;
            }
            if (!ConvertUtilities.Compare(pageLog.DisableNavigationCascade, pageLogModel.DisableNavigationCascade))
            {
                changeLog.AppendFormat(format, "DisableNavigationCascade");
                pageLog.DisableNavigationCascade = pageLogModel.DisableNavigationCascade;
            }
            if (!ConvertUtilities.Compare(pageLog.StartPublishingDate, pageLogModel.StartPublishingDate))
            {
                changeLog.AppendFormat(format, "StartPublishingDate");
                pageLog.StartPublishingDate = pageLogModel.StartPublishingDate;
            }
            if (!ConvertUtilities.Compare(pageLog.EndPublishingDate, pageLogModel.EndPublishingDate))
            {
                changeLog.AppendFormat(format, "EndPublishingDate");
                pageLog.EndPublishingDate = pageLogModel.EndPublishingDate;
            }

            if (!string.IsNullOrEmpty(changeLog.ToString()))
            {
                changeLog.Insert(0, "** Update Page **\n");
            }

            return changeLog.ToString();
        }

        #endregion
    }
}