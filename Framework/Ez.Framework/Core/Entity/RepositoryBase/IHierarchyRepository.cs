using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Configurations;
using Ez.Framework.Core.Entity.Models;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ez.Framework.Core.Entity.RepositoryBase
{
    /// <summary>
    /// Hierarchy repository interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHierarchyRepository<T> : IRepository<T> where T : BaseHierachyModel<T>
    {
        /// <summary>
        /// Insert entity with hierarchy support
        /// </summary>
        /// <param name="entity">the input entity</param>
        /// <returns></returns>
        ResponseModel HierarchyInsert(T entity);

        /// <summary>
        /// Update entity with hierarchy support 
        /// </summary>
        /// <param name="entity">the input entity</param>
        /// <returns></returns>
        ResponseModel HierarchyUpdate(T entity);

        /// <summary>
        /// Delete entity with hierarchy support
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ResponseModel HierarchyDelete(T entity);

        /// <summary>
        /// Get possible parents
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        IQueryable<T> GetPossibleParents(T entity);

        /// <summary>
        /// Get all child items of parent
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includeRoot"></param>
        /// <param name="orderAsc"></param>
        /// <returns></returns>
        IQueryable<T> GetHierarchies(T entity, bool includeRoot = true, bool orderAsc = true);

        /// <summary>
        /// Get all parents of item
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includeChild"></param>
        /// <param name="orderAsc"></param>
        /// <returns></returns>
        IQueryable<T> GetParents(T entity, bool includeChild, bool orderAsc = true);

        /// <summary>
        /// Get all parents of item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeChild"></param>
        /// <param name="orderAsc"></param>
        /// <returns></returns>
        IQueryable<T> GetParents(int? id, bool includeChild, bool orderAsc = true);

        /// <summary>
        /// Get all child items of parent
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        IQueryable<T> GetHierarchies(List<T> entities);

        /// <summary>
        /// Check if root item has same order or not (if not it will causing issue when create dropdown)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        bool IsLevelOrderExisted(int? id, int? parentId, int order);

        /// <summary>
        /// Build select list from data
        /// </summary>
        /// <param name="data">the input data must be serializable</param>
        /// <param name="levelPrefix">the prefix level</param>
        /// <param name="needReorder"> </param>
        /// <returns></returns>
        List<SelectListItem> BuildSelectList(List<HierarchyDropdownModel> data, bool needReorder = true, string levelPrefix = FrameworkConstants.HierarchyLevelPrefix);
    }
}