using Ez.Framework.Configurations;
using Ez.Framework.Core.Entity.Models;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Ez.Framework.Core.Entity.RepositoryBase
{
    /// <summary>
    /// Base repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : BaseModel
    {
        #region Protected Properties

        protected DbContext DataContext { get; set; }

        #endregion

        public DbContext DbContext
        {
            get { return DataContext; }
        }

        public Repository(DbContext entities)
        {
            DataContext = entities;
        }

        #region Public Methods

        /// <summary>
        /// Get current db context
        /// </summary>
        /// <returns></returns>
        public DbContext GetDbContext()
        {
            return DbContext;
        }

        /// <summary>
        /// Get the connection
        /// </summary>
        /// <returns></returns>
        public virtual DbConnection Connection()
        {
            return DataContext.Database.Connection;
        }

        #region Get

        public virtual IQueryable<T> GetAll()
        {
            return DataContext.Set<T>().Where(i => !i.RecordDeleted).OrderBy(i => i.RecordOrder);
        }

        public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> expression)
        {
            return GetAll().Where(expression).OrderBy(i => i.RecordOrder);
        }

        public virtual T FetchFirst(Expression<Func<T, bool>> expression)
        {
            return GetAll().FirstOrDefault(expression);
        }

        public virtual T GetById(object id)
        {
            var entity = DataContext.Set<T>().Find(id);
            if (entity != null && entity.RecordDeleted) return default(T);

            return entity;
        }

        public virtual T GetById(int id)
        {
            return DataContext.Set<T>().FirstOrDefault(i => i.Id == id);
        }

        #endregion

        /// <summary>
        /// Excute sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual ResponseModel ExcuteSql(string sql)
        {
            var response = new ResponseModel();
            try
            {
                DataContext.Database.ExecuteSqlCommand(sql);
                DataContext.SaveChanges();
                response.Success = true;
            }
            catch (DbEntityValidationException entityValidationException)
            {
                response = new ResponseModel(entityValidationException);
            }
            catch (Exception exception)
            {
                response = new ResponseModel(exception);
            }
            return response;
        }

        #region Insert

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ResponseModel Insert(T entity)
        {
            var now = DateTime.UtcNow;
            entity.LastUpdate = entity.Created = now;
            entity.LastUpdateBy = entity.CreatedBy = (HttpContext.Current != null && HttpContext.Current.User != null && !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)
                ? HttpContext.Current.User.Identity.Name
                : FrameworkConstants.DefaultSystemAccount).SafeSubstring(100);

            entity.RecordDeleted = false;

            var response = new ResponseModel();
            try
            {
                var dbSet = DataContext.Set<T>();
                dbSet.Add(entity);
                DataContext.SaveChanges();
                response.Data = entity.Id;
                response.Success = true;
            }
            catch (DbEntityValidationException entityValidationException)
            {
                response = new ResponseModel(entityValidationException);
            }
            catch (Exception exception)
            {
                response = new ResponseModel(exception);
            }
            return response;
        }

        /// <summary>
        /// Insert list of entity
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual ResponseModel Insert(IEnumerable<T> entities)
        {
            var response = new ResponseModel();
            var now = DateTime.UtcNow;
            try
            {
                var dbSet = DataContext.Set<T>();
                foreach (var entity in entities)
                {
                    entity.Created = now;
                    entity.CreatedBy = (HttpContext.Current != null && HttpContext.Current.User != null && !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)
                        ? HttpContext.Current.User.Identity.Name
                        : FrameworkConstants.DefaultSystemAccount).SafeSubstring(100);
                    entity.LastUpdate = now;
                    entity.LastUpdateBy = (HttpContext.Current != null && HttpContext.Current.User != null && !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)
                        ? HttpContext.Current.User.Identity.Name
                        : FrameworkConstants.DefaultSystemAccount).SafeSubstring(100);

                    entity.RecordDeleted = false;
                    dbSet.Add(entity);
                }
                DataContext.SaveChanges();
                response.Success = true;
            }
            catch (DbEntityValidationException entityValidationException)
            {
                response = new ResponseModel(entityValidationException);
            }
            catch (Exception exception)
            {
                response = new ResponseModel(exception);
            }
            return response;
        }

        #endregion

        #region Delete

        public virtual ResponseModel Delete(T entity)
        {
            var response = new ResponseModel();

            if (entity == null)
            {
                response.Success = true;
                return response;
            }

            try
            {
                var dbSet = DataContext.Set<T>();
                dbSet.Remove(entity);
                DataContext.SaveChanges();
                response.Success = true;
            }
            catch (DbEntityValidationException entityValidationException)
            {
                response = new ResponseModel(entityValidationException);
            }
            catch (Exception exception)
            {
                response = new ResponseModel(exception);
            }
            return response;
        }

        public virtual ResponseModel Delete(IEnumerable<T> entities)
        {
            var response = new ResponseModel();

            if (entities == null || !entities.Any())
            {
                response.Success = true;
                return response;
            }

            try
            {
                var dbSet = DataContext.Set<T>();
                dbSet.RemoveRange(entities);
                DataContext.SaveChanges();
                response.Success = true;
            }
            catch (DbEntityValidationException entityValidationException)
            {
                response = new ResponseModel(entityValidationException);
            }
            catch (Exception exception)
            {
                response = new ResponseModel(exception);
            }
            return response;
        }

        public virtual ResponseModel Delete(IEnumerable<int> ids)
        {
            var response = new ResponseModel();

            if (ids == null || !ids.Any())
            {
                response.Success = true;
                return response;
            }

            try
            {
                var dbSet = DataContext.Set<T>();
                dbSet.RemoveRange(Fetch(e => ids.Contains(e.Id)));
                DataContext.SaveChanges();
                response.Success = true;
            }
            catch (DbEntityValidationException entityValidationException)
            {
                response = new ResponseModel(entityValidationException);
            }
            catch (Exception exception)
            {
                response = new ResponseModel(exception);
            }
            return response;
        }

        public virtual ResponseModel Delete(object id)
        {
            var response = new ResponseModel();
            try
            {
                var entity = GetById(id);
                var dbSet = DataContext.Set<T>();
                dbSet.Remove(entity);
                DataContext.SaveChanges();
                response.Success = true;
            }
            catch (DbEntityValidationException entityValidationException)
            {
                response = new ResponseModel(entityValidationException);
            }
            catch (Exception exception)
            {
                response = new ResponseModel(exception);
            }
            return response;
        }

        #endregion

        #region Update

        public virtual ResponseModel Update(T entity)
        {
            var response = new ResponseModel();
            entity.LastUpdate = DateTime.UtcNow;
            entity.LastUpdateBy = (HttpContext.Current != null && HttpContext.Current.User != null && !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)
                ? HttpContext.Current.User.Identity.Name
                : FrameworkConstants.DefaultSystemAccount).SafeSubstring(100);
            try
            {
                DataContext.SaveChanges();
                response.Data = entity.Id;
                response.Success = true;
            }
            catch (DbEntityValidationException entityValidationException)
            {
                response = new ResponseModel(entityValidationException);
            }
            catch (Exception exception)
            {
                response = new ResponseModel(exception);
            }
            return response;
        }

        public virtual ResponseModel SetRecordDeleted(int id)
        {
            ResponseModel response;
            try
            {
                var entity = GetById(id);
                entity.RecordDeleted = true;
                return Update(entity);
            }
            catch (DbEntityValidationException entityValidationException)
            {
                response = new ResponseModel(entityValidationException);
            }
            catch (Exception exception)
            {
                response = new ResponseModel(exception);
            }
            return response;
        }

        #endregion

        #endregion
    }
}