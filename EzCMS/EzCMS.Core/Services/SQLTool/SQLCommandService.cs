using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.Mvc.Helpers;
using EzCMS.Core.Models.SQLTool;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace EzCMS.Core.Services.SQLTool
{
    public class SQLCommandService : ServiceHelper, ISQLCommandService
    {
        private readonly IRepository<SQLCommandHistory> _sqlCommandHistoryRepository;

        public SQLCommandService(IRepository<SQLCommandHistory> sqlCommandHistoryRepository)
        {
            _sqlCommandHistoryRepository = sqlCommandHistoryRepository;
        }

        #region Grid

        /// <summary>
        /// Search the testimonials
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchCommands(JqSearchIn si)
        {
            var testimonials = GetAll().Select(c => new SqlCommandHistoryModel
            {
                Id = c.Id,
                Query = c.Query,
                RecordOrder = c.RecordOrder,
                Created = c.Created,
                CreatedBy = c.CreatedBy,
                LastUpdate = c.LastUpdate,
                LastUpdateBy = c.LastUpdateBy
            });

            return si.Search(testimonials);
        }

        #endregion

        /// <summary>
        /// Get connection
        /// </summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            return _sqlCommandHistoryRepository.Connection();
        }

        /// <summary>
        /// Get connection string
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            return _sqlCommandHistoryRepository.Connection().ConnectionString;
        }

        /// <summary>
        /// Save a SQL request into history for later use
        /// </summary>
        /// <param name="request"></param>
        public ResponseModel SaveCommand(SQLRequest request)
        {
            if (request.Query == null)
            {
                return new ResponseModel
                {
                    Success = true
                };
            }
            var last = GetLastCommand();
            if (last != null && last.Query == request.Query)
            {
                return new ResponseModel
                {
                    Success = true
                };
            }
            var history = new SqlCommandHistoryModel
            {
                Query = request.Query
            };
            var response = Insert(history);

            return response.SetMessage(response.Success
                ? T("SqlCommand_Message_CreateSuccessfully")
                : T("SqlCommand_Message_CreateFailure"));
        }

        /// <summary>
        /// Get history request for current user
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<SqlCommandHistoryModel> GetHistories(int index, int pageSize)
        {
            var username = HttpContext.Current.User.Identity.Name;
            return Fetch(i => i.CreatedBy.Equals(username))
                .OrderByDescending(i => i.Created)
                .Skip(index)
                .Take(pageSize)
                .Select(
                    i => new SqlCommandHistoryModel { Id = i.Id, Query = i.Query, CreatedBy = i.CreatedBy });
        }

        internal ResponseModel Insert(SqlCommandHistoryModel historyModel)
        {
            var commandHistory = new SQLCommandHistory
            {
                Query = historyModel.Query
            };
            return Insert(commandHistory);
        }

        /// <summary>
        /// Get last command
        /// </summary>
        /// <returns></returns>
        public SqlCommandHistoryModel GetLastCommand()
        {
            return GetHistories(0, 1).FirstOrDefault();
        }

        #region Base

        public IQueryable<SQLCommandHistory> GetAll()
        {
            return _sqlCommandHistoryRepository.GetAll();
        }

        public IQueryable<SQLCommandHistory> Fetch(Expression<Func<SQLCommandHistory, bool>> expression)
        {
            return _sqlCommandHistoryRepository.Fetch(expression);
        }

        public SQLCommandHistory FetchFirst(Expression<Func<SQLCommandHistory, bool>> expression)
        {
            return _sqlCommandHistoryRepository.FetchFirst(expression);
        }

        public SQLCommandHistory GetById(object id)
        {
            return _sqlCommandHistoryRepository.GetById(id);
        }

        internal ResponseModel Insert(SQLCommandHistory sqlCommandHistory)
        {
            return _sqlCommandHistoryRepository.Insert(sqlCommandHistory);
        }

        internal ResponseModel Update(SQLCommandHistory sqlCommandHistory)
        {
            return _sqlCommandHistoryRepository.Update(sqlCommandHistory);
        }

        internal ResponseModel Delete(SQLCommandHistory sqlCommandHistory)
        {
            return _sqlCommandHistoryRepository.Delete(sqlCommandHistory);
        }

        internal ResponseModel Delete(object id)
        {
            return _sqlCommandHistoryRepository.Delete(id);
        }

        #endregion
    }
}