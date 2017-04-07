using Ez.Framework.Configurations;
using Ez.Framework.Core.Entity.Models;
using Ez.Framework.Core.Entity.RepositoryBase.Extensions;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Ez.Framework.Core.Entity.RepositoryBase
{
    /// <summary>
    /// Hierarchy repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HierarchyRepository<T> : Repository<T>, IHierarchyRepository<T> where T : BaseHierachyModel<T>
    {
        public HierarchyRepository(DbContext entities)
            : base(entities)
        {
        }

        #region Hierarchy Repository

        /// <summary>
        /// Insert entity with hierarchy support
        /// </summary>
        /// <param name="entity">the input entity</param>
        /// <returns></returns>
        public virtual ResponseModel HierarchyInsert(T entity)
        {
            entity.Hierarchy = string.Empty;
            var response = Insert(entity);
            if (response.Success)
            {
                var parentId = entity.ParentId;
                var parent = GetById(parentId);
                var id = entity.Id;
                if (parent != null)
                {
                    var menuHierarchy = parent.Hierarchy;
                    entity.Hierarchy = string.Format("{0}{1}{2}", menuHierarchy, id.ToString(FrameworkConstants.HierarchyNodeFormat), FrameworkConstants.IdSeparator);
                }
                else
                {
                    entity.Hierarchy = string.Format("{0}{1}{0}", FrameworkConstants.IdSeparator, id.ToString(FrameworkConstants.HierarchyNodeFormat));
                }

                return Update(entity);
            }
            return response;
        }

        /// <summary>
        /// Update entity with hierarchy support 
        /// </summary>
        /// <param name="entity">the input entity</param>
        /// <returns></returns>
        public virtual ResponseModel HierarchyUpdate(T entity)
        {
            var entry = DataContext.Entry(entity);
            if (!Equals(entry.OriginalValues[FrameworkConstants.ParentIdPropertyName], entry.CurrentValues[FrameworkConstants.ParentIdPropertyName]))
            {
                var tableName = DataContext.GetTableName<T>();

                var parentId = (int?)entry.CurrentValues[FrameworkConstants.ParentIdPropertyName];
                var parent = GetById(parentId);

                var currentPrefix = entity.Hierarchy;
                var hierarchy = parent == null ? entity.GetHierarchyValueForRoot() : entity.CalculateHierarchyValue(parent);

                var query = string.Format("UPDATE " + tableName +
                                          " SET {2} = '{1}' + RIGHT({2}, LEN({2}) - LEN('{0}')) " +
                                          " WHERE {2} LIKE '{0}%'"
                    , currentPrefix, hierarchy, FrameworkConstants.HierarchyPropertyName);
                DataContext.Database.ExecuteSqlCommand(query);
            }
            return Update(entity);
        }

        /// <summary>
        /// Delete entity with hierarchy support
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ResponseModel HierarchyDelete(T entity)
        {
            var response = new ResponseModel();
            return response;
        }

        /// <summary>
        /// Get possible parents
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetPossibleParents(T entity)
        {
            if (entity == null)
            {
                return null;
            }
            var prefix = entity.Hierarchy;
            return GetAll().Where(string.Format("!{0}.Contains(\"{1}\")", FrameworkConstants.HierarchyPropertyName, prefix));
        }

        /// <summary>
        /// Get all child items of parent
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includeRoot"></param>
        /// <param name="orderAsc"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetHierarchies(T entity, bool includeRoot = true, bool orderAsc = true)
        {
            if (entity == null)
            {
                return null;
            }
            var prefix = entity.Hierarchy;
            var data = GetAll();
            if (!includeRoot)
            {
                data = Fetch(p => p.Id != entity.Id);
            }
            var hierarchies = data.Where(string.Format("{0}.StartsWith(\"{1}\")", FrameworkConstants.HierarchyPropertyName, prefix));

            if (orderAsc) return hierarchies.OrderBy(p => p.Hierarchy);
            return hierarchies.OrderByDescending(p => p.Hierarchy);
        }

        /// <summary>
        /// Get hieracies
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public IQueryable<T> GetHierarchies(List<T> entities)
        {
            if (entities.Any())
            {
                var whereClause = string.Join(" OR ", entities.Select(entity => string.Format("{0}.StartsWith(\"{1}\")", FrameworkConstants.HierarchyPropertyName, entity.Hierarchy)));
                return GetAll().Where(whereClause);
            }
            return null;
        }

        /// <summary>
        /// Get all parents of item (and include child if needed)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includeChild"></param>
        /// <param name="orderAsc"></param>
        /// <returns></returns>
        public IQueryable<T> GetParents(T entity, bool includeChild, bool orderAsc = true)
        {
            if (entity == null)
            {
                return null;
            }
            var suffix = entity.Hierarchy;
            var parents = Fetch(p => includeChild || p.Id != entity.Id).Where(string.Format("\"{0}\".StartsWith({1})", suffix, FrameworkConstants.HierarchyPropertyName));
            if (orderAsc) return parents.OrderBy(p => p.Hierarchy);
            return parents.OrderByDescending(p => p.Hierarchy);
        }

        /// <summary>
        /// Get all parents of item (and include child if needed)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeChild"></param>
        /// <param name="orderAsc"></param>
        /// <returns></returns>
        public IQueryable<T> GetParents(int? id, bool includeChild, bool orderAsc = true)
        {
            var entity = GetById(id);
            if (entity == null)
            {
                return null;
            }
            var suffix = entity.Hierarchy;
            var parents = Fetch(p => includeChild || p.Id != entity.Id).Where(string.Format("\"{0}\".StartsWith({1})", suffix, FrameworkConstants.HierarchyPropertyName));
            if (orderAsc) return parents.OrderBy(p => p.Hierarchy);
            return parents.OrderByDescending(p => p.Hierarchy);
        }

        /// <summary>
        /// Check if root item has same order or not (if not it will causing issue when create dropdown)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual bool IsLevelOrderExisted(int? id, int? parentId, int order)
        {
            return Fetch(m => m.Id != id && m.RecordOrder == order
                && ((!parentId.HasValue && !m.ParentId.HasValue) || m.ParentId == parentId)).Any();
        }

        /// <summary>
        /// Build select list from data
        /// </summary>
        /// <param name="data">the input data must be serializable</param>
        /// <param name="levelPrefix">the prefix level</param>
        /// <param name="needReorder"> </param>
        /// <returns></returns>
        public virtual List<SelectListItem> BuildSelectList(List<HierarchyDropdownModel> data, bool needReorder = true, string levelPrefix = FrameworkConstants.HierarchyLevelPrefix)
        {
            if (needReorder)
            {
                var dictionary = data.ToDictionary(i => i.Id, i => i.RecordOrder);
                foreach (var item in data)
                {
                    var hierarchy = item.Hierarchy;

                    var hierarchyIds = new List<int>();
                    if (!string.IsNullOrEmpty(hierarchy))
                    {
                        hierarchyIds =
                            hierarchy.Substring(1, hierarchy.Length - 2).Split(FrameworkConstants.IdSeparator).Select(int.Parse).ToList();
                    }

                    var order = string.Empty;
                    foreach (var id in hierarchyIds)
                    {
                        int? value;
                        if (dictionary.TryGetValue(id, out value))
                        {
                            if (value.HasValue)
                            {
                                order += string.Format("{0}{1}", FrameworkConstants.IdSeparator, value.Value.ToString(FrameworkConstants.HierarchyNodeFormat));
                            }
                        }
                    }
                    item.Hierarchy = order;
                }
                data.Sort(((i, j) => Comparer<string>.Default.Compare(i.Hierarchy, j.Hierarchy)));
            }

            var selectList = new List<SelectListItem>();
            foreach (var item in data)
            {
                var prefix = string.Empty;
                var hierarchy = item.Hierarchy;
                var count = hierarchy.Count(c => c.Equals(FrameworkConstants.IdSeparator));
                for (var i = 0; i < count - 1; i++)
                {
                    prefix += levelPrefix;
                }
                selectList.Add(new SelectListItem
                {
                    Text = string.Format("{0}{1}", prefix, item.Name),
                    Value = item.Id.ToString(CultureInfo.InvariantCulture),
                    Selected = item.Selected
                });
            }
            return selectList;
        }
        #endregion
    }
}
