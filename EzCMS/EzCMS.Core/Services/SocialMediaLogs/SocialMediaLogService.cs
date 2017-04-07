using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.SocialMediaLogs;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.SocialMediaLogs
{
    public class SocialMediaLogService : ServiceHelper, ISocialMediaLogService
    {
        private readonly IRepository<SocialMediaLog> _socialMediaLogRepository;

        public SocialMediaLogService(IRepository<SocialMediaLog> socialMediaLogRepository)
        {
            _socialMediaLogRepository = socialMediaLogRepository;
        }

        /// <summary>
        /// Save social media log
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveSocialMediaLog(SocialMediaLog model)
        {
            ResponseModel response;
            var socialMediaTokenLog = GetById(model.Id);
            if (socialMediaTokenLog != null)
            {
                socialMediaTokenLog.SocialMediaId = model.SocialMediaId;
                socialMediaTokenLog.PageId = model.PageId;
                socialMediaTokenLog.PostedContent = model.PostedContent;
                socialMediaTokenLog.PostedResponse = model.PostedResponse;
                socialMediaTokenLog.SocialMediaId = model.SocialMediaId;
                socialMediaTokenLog.SocialMediaTokenId = model.SocialMediaTokenId;
                response = Update(socialMediaTokenLog);
                return response.SetMessage(response.Success
                    ? T("SocialMediaLog_Message_UpdateSuccessfully")
                    : T("SocialMediaLog_Message_UpdateFailure"));
            }
            response = Insert(model);
            return response.SetMessage(response.Success
                ? T("SocialMediaLog_Message_CreateSuccessfully")
                : T("SocialMediaLog_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete social media logs older than date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public ResponseModel DeleteSocialMediaLogs(DateTime date)
        {
            var logs = Fetch(l => l.Created < date);
            return _socialMediaLogRepository.Delete(logs);
        }

        public ResponseModel DeleteSocialMediaLogs(IEnumerable<SocialMediaLog> logs)
        {
            return Delete(logs);
        }

        #region Base

        public IQueryable<SocialMediaLog> GetAll()
        {
            return _socialMediaLogRepository.GetAll();
        }

        public IQueryable<SocialMediaLog> Fetch(Expression<Func<SocialMediaLog, bool>> expression)
        {
            return _socialMediaLogRepository.Fetch(expression);
        }

        public SocialMediaLog FetchFirst(Expression<Func<SocialMediaLog, bool>> expression)
        {
            return _socialMediaLogRepository.FetchFirst(expression);
        }

        public SocialMediaLog GetById(object id)
        {
            return _socialMediaLogRepository.GetById(id);
        }

        public ResponseModel Insert(SocialMediaLog socialMediaLog)
        {
            return _socialMediaLogRepository.Insert(socialMediaLog);
        }

        internal ResponseModel Update(SocialMediaLog socialMediaLog)
        {
            return _socialMediaLogRepository.Update(socialMediaLog);
        }

        internal ResponseModel Delete(SocialMediaLog socialMediaLog)
        {
            return _socialMediaLogRepository.Delete(socialMediaLog);
        }

        internal ResponseModel Delete(object id)
        {
            return _socialMediaLogRepository.Delete(id);
        }

        internal ResponseModel Delete(IEnumerable<SocialMediaLog> entities)
        {
            return _socialMediaLogRepository.Delete(entities);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _socialMediaLogRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the social media logs
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchSocialMediaLogs(JqSearchIn si, int? socialMediaTokenId, int? socialMediaId)
        {
            var data = SearchSocialMediaLogs(socialMediaTokenId, socialMediaId);

            var socialMediaLogs = Maps(data);

            return si.Search(socialMediaLogs);
        }

        /// <summary>
        /// Export social media logs
        /// </summary>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? socialMediaTokenId,
            int? socialMediaId)
        {
            var data = gridExportMode == GridExportMode.All
                ? GetAll()
                : SearchSocialMediaLogs(socialMediaTokenId, socialMediaId);

            var socialMediaLogs = Maps(data);

            var exportData = si.Export(socialMediaLogs, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search social media logs
        /// </summary>
        /// <param name="socialMediaTokenId"></param>
        /// <param name="socialMediaId"></param>
        /// <returns></returns>
        private IQueryable<SocialMediaLog> SearchSocialMediaLogs(int? socialMediaTokenId, int? socialMediaId)
        {
            return
                Fetch(
                    socialMediaLog =>
                        (!socialMediaTokenId.HasValue || socialMediaLog.SocialMediaTokenId == socialMediaTokenId)
                        && (!socialMediaId.HasValue || socialMediaLog.SocialMediaToken.SocialMediaId == socialMediaId));
        }

        /// <summary>
        /// Map social media logs
        /// </summary>
        /// <param name="socialMediaLogs"></param>
        /// <returns></returns>
        private IQueryable<SocialMediaLogModel> Maps(IQueryable<SocialMediaLog> socialMediaLogs)
        {
            return socialMediaLogs.Select(m => new SocialMediaLogModel
            {
                Id = m.Id,
                PageTitle = m.Page.Title,
                SocialMedia = m.SocialMediaToken.SocialMedia.Name,
                SocialMediaToken = m.SocialMediaToken.FullName,
                PostedContent = m.PostedContent,
                PostedResponse = m.PostedResponse,
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