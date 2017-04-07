using Ez.Framework.Core.Entity.Models;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Ez.Framework.Core.Entity.RepositoryBase
{
    /// <summary>
    /// Base repository interface
    /// </summary>
    /// <typeparam name="T">the entity type</typeparam>
    public interface IRepository<T> where T : BaseModel
    {
        DbContext GetDbContext();

        DbConnection Connection();

        #region Get

        IQueryable<T> GetAll();

        IQueryable<T> Fetch(Expression<Func<T, bool>> expression);

        T FetchFirst(Expression<Func<T, bool>> expression);

        T GetById(object id);

        T GetById(int id);

        ResponseModel ExcuteSql(string sql);

        #endregion

        #region Insert

        ResponseModel Insert(T entity);

        ResponseModel Insert(IEnumerable<T> entities);

        #endregion

        #region Delete

        ResponseModel Delete(T entity);

        ResponseModel Delete(IEnumerable<T> entities);

        ResponseModel Delete(IEnumerable<int> ids);

        ResponseModel Delete(object id);

        #endregion

        #region Update

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ResponseModel Update(T entity);

        /// <summary>
        /// Set record deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel SetRecordDeleted(int id);

        #endregion
    }
}
