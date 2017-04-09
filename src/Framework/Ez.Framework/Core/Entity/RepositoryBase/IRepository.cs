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

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Fetch entities
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IQueryable<T> Fetch(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Fetch first entity
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        T FetchFirst(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(object id);

        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(int id);

        /// <summary>
        /// Execute sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        ResponseModel ExcuteSql(string sql);

        #endregion

        #region Insert

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ResponseModel Insert(T entity);

        /// <summary>
        /// Insert list of entities
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        ResponseModel Insert(IEnumerable<T> entities);

        #endregion

        #region Delete

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ResponseModel Delete(T entity);

        /// <summary>
        /// Delete list of entities
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        ResponseModel Delete(IEnumerable<T> entities);

        /// <summary>
        /// Delete list entity ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        ResponseModel Delete(IEnumerable<int> ids);

        /// <summary>
        /// Delete entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
