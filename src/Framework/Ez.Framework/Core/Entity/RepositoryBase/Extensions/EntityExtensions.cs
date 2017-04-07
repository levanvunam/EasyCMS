using Ez.Framework.Configurations;
using Ez.Framework.Utilities.Reflection;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;

namespace Ez.Framework.Core.Entity.RepositoryBase.Extensions
{
    public static class EntityExtensions
    {

        #region Get table name
        public static string GetTableName<T>(this DbContext context) where T : class
        {
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;

            return objectContext.GetTableName<T>();
        }

        public static string GetTableName<T>(this ObjectContext context) where T : class
        {
            string sql = context.CreateObjectSet<T>().ToTraceString();
            var regex = new Regex("FROM (?<table>.*) AS");
            Match match = regex.Match(sql);

            string table = match.Groups["table"].Value;
            return table;
        }

        #endregion

        /// <summary>
        /// Get Id property of entity
        /// </summary>
        /// <typeparam name="T">the entity type</typeparam>
        /// <param name="entity">the entity</param>
        /// <returns></returns>
        public static int GetId<T>(this T entity)
        {
            return (int)entity.GetPropertyValue(FrameworkConstants.IdPropertyName);
        }

        /// <summary>
        /// Get Parent Id property of entity
        /// </summary>
        /// <typeparam name="T">the entity type</typeparam>
        /// <param name="entity">the entity</param>
        /// <returns></returns>
        public static int? GetParentId<T>(this T entity)
        {
            return (int?)entity.GetPropertyValue(FrameworkConstants.ParentIdPropertyName);
        }

        /// <summary>
        /// Get Hierarchy property of entity
        /// </summary>
        /// <typeparam name="T">the entity type</typeparam>
        /// <param name="entity">the entity</param>
        /// <returns></returns>
        public static string GetHierarchy<T>(this T entity)
        {
            return (string)entity.GetPropertyValue(FrameworkConstants.HierarchyPropertyName);
        }

        /// <summary>
        /// Get new hierarchy string
        /// </summary>
        /// <typeparam name="T">the entity type</typeparam>
        /// <param name="item">the item</param>
        /// <param name="parent">the parent item</param>
        /// <returns></returns>
        public static string CalculateHierarchyValue<T>(this T item, T parent)
        {
            return string.Concat(parent.GetHierarchy(), item.GetId().ToString(FrameworkConstants.HierarchyNodeFormat), FrameworkConstants.IdSeparator);
        }

        /// <summary>
        /// Get hierarchy item of entity
        /// </summary>
        /// <typeparam name="T">the entity type</typeparam>
        /// <param name="item">the item</param>
        /// <returns></returns>
        public static string GetHierarchyValueForRoot<T>(this T item)
        {
            return string.Concat(FrameworkConstants.IdSeparator, item.GetId().ToString(FrameworkConstants.HierarchyNodeFormat), FrameworkConstants.IdSeparator);
        }

        /// <summary>
        /// Get hierarchy item of entity with id
        /// </summary>
        /// <param name="item">the item id</param>
        /// <returns></returns>
        public static string GetHierarchyValueForRoot(this int item)
        {
            return string.Concat(FrameworkConstants.IdSeparator, item.ToString(FrameworkConstants.HierarchyNodeFormat), FrameworkConstants.IdSeparator);
        }
    }
}
