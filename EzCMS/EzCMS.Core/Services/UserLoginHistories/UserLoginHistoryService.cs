using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Web;
using Ez.Framework.Utilities.Web.Models;
using EzCMS.Core.Models.UserLoginHistories;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.UserLoginHistories
{
    public class UserLoginHistoryService : ServiceHelper, IUserLoginHistoryService
    {
        private readonly IRepository<UserLoginHistory> _userLoginHistoryRepository;

        public UserLoginHistoryService(IRepository<UserLoginHistory> userLoginHistoryRepository)
        {
            _userLoginHistoryRepository = userLoginHistoryRepository;
        }

        #region Manage

        /// <summary>
        /// Log login history for user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public ResponseModel InsertUserLoginHistory(int userId, HttpContext context)
        {
            var userAgentInformation = context.GetUserAgentInformationFromRequest();

            Mapper.CreateMap<UserAgentModel, UserLoginHistory>();
            var userLoginHistory = Mapper.Map<UserAgentModel, UserLoginHistory>(userAgentInformation);
            userLoginHistory.UserId = userId;

            return _userLoginHistoryRepository.Insert(userLoginHistory);
        }

        #endregion

        #region Details

        /// <summary>
        /// Get user login history detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserLoginHistoryDetailModel GetUserLoginHistoryDetailModel(int id)
        {
            var userLoginHistory = GetById(id);
            return userLoginHistory != null ? new UserLoginHistoryDetailModel(userLoginHistory) : null;
        }

        #endregion

        #region Base

        public IQueryable<UserLoginHistory> GetAll()
        {
            return _userLoginHistoryRepository.GetAll();
        }

        public IQueryable<UserLoginHistory> Fetch(Expression<Func<UserLoginHistory, bool>> expression)
        {
            return _userLoginHistoryRepository.Fetch(expression);
        }

        public UserLoginHistory FetchFirst(Expression<Func<UserLoginHistory, bool>> expression)
        {
            return _userLoginHistoryRepository.FetchFirst(expression);
        }

        public UserLoginHistory GetById(object id)
        {
            return _userLoginHistoryRepository.GetById(id);
        }

        public ResponseModel Insert(UserLoginHistory userLoginHistory)
        {
            return _userLoginHistoryRepository.Insert(userLoginHistory);
        }

        internal ResponseModel Update(UserLoginHistory userLoginHistory)
        {
            return _userLoginHistoryRepository.Update(userLoginHistory);
        }

        internal ResponseModel Delete(UserLoginHistory userLoginHistory)
        {
            return _userLoginHistoryRepository.Delete(userLoginHistory);
        }

        internal ResponseModel Delete(object id)
        {
            return _userLoginHistoryRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _userLoginHistoryRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the user login histories.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchUserLoginHistories(JqSearchIn si, UserLoginHistorySearchModel model)
        {
            var data = SearchUserLoginHistories(model);

            var userLoginHistories = Maps(data);

            return si.Search(userLoginHistories);
        }

        /// <summary>
        /// Export user login histories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, UserLoginHistorySearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchUserLoginHistories(model);

            var userLoginHistories = Maps(data);

            var exportData = si.Export(userLoginHistories, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search user login histories
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<UserLoginHistory> SearchUserLoginHistories(UserLoginHistorySearchModel model)
        {
            return Fetch(userLoginHistory => (string.IsNullOrEmpty(model.Keyword)
                                              ||
                                              (!string.IsNullOrEmpty(userLoginHistory.User.Username) &&
                                               userLoginHistory.User.Username.Contains(model.Keyword))
                                              ||
                                              (!string.IsNullOrEmpty(userLoginHistory.User.Email) &&
                                               userLoginHistory.User.Email.Contains(model.Keyword))
                                              ||
                                              (!string.IsNullOrEmpty(userLoginHistory.BrowserName) &&
                                               userLoginHistory.BrowserName.Contains(model.Keyword))
                                              ||
                                              (!string.IsNullOrEmpty(userLoginHistory.BrowserType) &&
                                               userLoginHistory.BrowserType.Contains(model.Keyword))
                                              ||
                                              (!string.IsNullOrEmpty(userLoginHistory.BrowserVersion) &&
                                               userLoginHistory.BrowserVersion.Contains(model.Keyword))
                                              ||
                                              (!string.IsNullOrEmpty(userLoginHistory.IpAddress) &&
                                               userLoginHistory.IpAddress.Contains(model.Keyword))
                                              ||
                                              (!string.IsNullOrEmpty(userLoginHistory.Platform) &&
                                               userLoginHistory.Platform.Contains(model.Keyword))
                                              ||
                                              (!string.IsNullOrEmpty(userLoginHistory.OsVersion) &&
                                               userLoginHistory.OsVersion.Contains(model.Keyword)))
                                             && (!model.UserId.HasValue || userLoginHistory.UserId == model.UserId));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="userLoginHistories"></param>
        /// <returns></returns>
        private IQueryable<UserLoginHistoryModel> Maps(IQueryable<UserLoginHistory> userLoginHistories)
        {
            return userLoginHistories.Select(userLoginHistory => new UserLoginHistoryModel
            {
                Id = userLoginHistory.Id,
                UserId = userLoginHistory.UserId,
                Username =
                    userLoginHistory.UserId.HasValue
                        ? (string.IsNullOrEmpty(userLoginHistory.User.Username)
                            ? userLoginHistory.User.Email
                            : userLoginHistory.User.Username)
                        : string.Empty,
                IpAddress = userLoginHistory.IpAddress,
                OsVersion = userLoginHistory.OsVersion,
                BrowserName = userLoginHistory.BrowserName,
                BrowserType = userLoginHistory.BrowserType,
                BrowserVersion = userLoginHistory.BrowserVersion,
                Platform = userLoginHistory.Platform,
                MajorVersion = userLoginHistory.MajorVersion,
                IsBeta = userLoginHistory.IsBeta,
                IsCrawler = userLoginHistory.IsCrawler,
                IsAOL = userLoginHistory.IsAOL,
                IsWin16 = userLoginHistory.IsWin16,
                IsWin32 = userLoginHistory.IsWin32,
                SupportsFrames = userLoginHistory.SupportsFrames,
                SupportsTables = userLoginHistory.SupportsTables,
                SupportsCookies = userLoginHistory.SupportsCookies,
                SupportsVBScript = userLoginHistory.SupportsVBScript,
                SupportsJavaScript = userLoginHistory.SupportsJavaScript,
                SupportsJavaApplets = userLoginHistory.SupportsJavaApplets,
                JavaScriptVersion = userLoginHistory.JavaScriptVersion,
                RecordOrder = userLoginHistory.RecordOrder,
                Created = userLoginHistory.Created,
                CreatedBy = userLoginHistory.CreatedBy,
                LastUpdate = userLoginHistory.LastUpdate,
                LastUpdateBy = userLoginHistory.LastUpdateBy
            });
        }

        #endregion

        #endregion
    }
}