using Ez.Framework.Configurations;
using Ez.Framework.Core.Entity.Models;
using Ez.Framework.Core.Entity.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

namespace Ez.Framework.Core.Entity.Extensions
{
    public static class MigrationExtensions
    {
        /// <summary>
        /// Run raw sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="nameOrConnectionString"></param>
        public static void Run(this string sql, string nameOrConnectionString)
        {
            using (var scope = new TransactionScope())
            {
                var sqlqueries = sql.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
                var con = new SqlConnection(nameOrConnectionString);
                var cmd = new SqlCommand("", con);
                con.Open();
                foreach (var query in sqlqueries)
                {
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
                con.Close();
                scope.Complete();
            }
        }

        #region Add entities

        /// <summary>
        /// Add entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="set"></param>
        /// <param name="entities"></param>
        public static void AddEntities<TEntity>(this DbSet<TEntity> set, params TEntity[] entities) where TEntity : BaseModel
        {
            foreach (var entity in entities)
            {
                entity.Created = DateTime.UtcNow;
                entity.CreatedBy = FrameworkConstants.DefaultMigrationAccount;
                entity.RecordDeleted = false;
                set.Add(entity);
            }
        }

        /// <summary>
        /// Add entity if not exists
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="set"></param>
        /// <param name="identifierExpression"></param>
        /// <param name="entities"></param>
        public static void AddIfNotExist<TEntity>(this DbSet<TEntity> set,
            Expression<Func<TEntity, object>> identifierExpression,
            params TEntity[] entities) where TEntity : BaseModel
        {

            foreach (var entity in entities)
            {
                var v = identifierExpression.Compile()(entity);
                Expression<Func<TEntity, bool>> predicate = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(identifierExpression.Body, Expression.Constant(v)), identifierExpression.Parameters);

                var entry = set.FirstOrDefault(predicate);
                if (entry == null)
                {
                    entity.Created = DateTime.UtcNow;
                    entity.CreatedBy = FrameworkConstants.DefaultMigrationAccount;
                    entity.RecordDeleted = false;
                    set.Add(entity);
                }
            }
        }

        /// <summary>
        /// Add entities if not exists
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="set"></param>
        /// <param name="identifierExpressions"></param>
        /// <param name="entities"></param>
        public static void AddIfNotExist<TEntity>(this DbSet<TEntity> set,
            List<Expression<Func<TEntity, object>>> identifierExpressions,
            params TEntity[] entities) where TEntity : BaseModel
        {

            foreach (var entity in entities)
            {
                IQueryable<TEntity> entries = set;
                foreach (var identifierExpression in identifierExpressions)
                {
                    var v = identifierExpression.Compile()(entity);
                    Expression<Func<TEntity, bool>> predicate = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(identifierExpression.Body, Expression.Constant(v)), identifierExpression.Parameters);
                    entries = entries.Where(predicate);
                }

                if (entries != null)
                {
                    var entry = entries.FirstOrDefault();
                    if (entry == null)
                    {
                        entity.Created = DateTime.UtcNow;
                        entity.CreatedBy = FrameworkConstants.DefaultMigrationAccount;
                        entity.RecordDeleted = false;
                        set.Add(entity);
                    }
                }
            }
        }

        /// <summary>
        /// Add entity if not exists
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="set"></param>
        /// <param name="identifierExpression"></param>
        /// <param name="entities"></param>
        public static void AddIfConditionInvalid<TEntity>(this DbSet<TEntity> set,
            Expression<Func<TEntity, bool>> identifierExpression,
            params TEntity[] entities) where TEntity : BaseModel
        {
            foreach (var entity in entities)
            {
                var entry = set.FirstOrDefault(identifierExpression);
                if (entry == null)
                {
                    entity.Created = DateTime.UtcNow;
                    entity.CreatedBy = FrameworkConstants.DefaultMigrationAccount;
                    entity.RecordDeleted = false;
                    set.Add(entity);
                }
            }
        }

        /// <summary>
        /// Add hierarchy entity if not exists
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="set"></param>
        /// <param name="identifierExpression"></param>
        /// <param name="entities"></param>
        public static void AddIfNotExist<TEntity>(this HierarchyRepository<TEntity> set,
            Expression<Func<TEntity, object>> identifierExpression,
            params TEntity[] entities) where TEntity : BaseHierachyModel<TEntity>
        {

            foreach (var entity in entities)
            {
                var v = identifierExpression.Compile()(entity);
                Expression<Func<TEntity, bool>> predicate = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(identifierExpression.Body, Expression.Constant(v)), identifierExpression.Parameters);

                var entry = set.FetchFirst(predicate);
                if (entry == null)
                {
                    entity.Created = DateTime.UtcNow;
                    entity.CreatedBy = FrameworkConstants.DefaultMigrationAccount;
                    entity.RecordDeleted = false;
                    set.HierarchyInsert(entity);
                }
            }
        }

        #endregion
    }
}
