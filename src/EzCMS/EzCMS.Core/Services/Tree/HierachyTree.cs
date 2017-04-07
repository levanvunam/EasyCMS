using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Extensions;
using Ez.Framework.Utilities.Reflection;

namespace EzCMS.Core.Services.Tree
{
    public class HierachyTree<T> : Tree<T>
    {
        public HierachyTree(T data)
        {
            Data = data;
            Children = new List<ITree<T>>();
        }

        public List<SelectListItem> GetSelectList(string levelPrefix, string textFieldName, IComparer<T> comparer,
            object excludeRootId = null, int depthLimnit = int.MaxValue)
        {
            return
                GetSelectList(
                    (item, lv) =>
                        string.Join("", Enumerable.Repeat(levelPrefix, lv - 1)) + item.GetPropertyValue(textFieldName),
                    comparer, excludeRootId, depthLimnit);
        }

        public List<SelectListItem> GetSelectList(Expression<Func<T, int, string>> textExpression, IComparer<T> comparer,
            object excludeRootId = null, int depthLimnit = int.MaxValue)
        {
            var result = new List<SelectListItem>();
            AddContentToSelectList(result, this, 1, textExpression.Compile(), comparer, excludeRootId, depthLimnit);
            return result;
        }

        public static List<HierachyTree<T>> CreateForest(IEnumerable<T> source)
        {
            var dataSource = new Dictionary<object, HierachyTree<T>>();

            foreach (var item in source)
            {
                object id = item.GetId();
                var node = new HierachyTree<T>(item);
                dataSource[id] = node;
            }

            foreach (var item in dataSource)
            {
                HierachyTree<T> parent;
                object parentId = item.Value.Data.GetParentId();

                if (parentId != null && dataSource.TryGetValue(parentId, out parent))
                {
                    item.Value.Parent = parent;
                    parent.Children.Add(item.Value);
                }
            }
            return dataSource.Values.Where(i => i.Parent == null).ToList();
        }

        public static HierachyTree<T> CreateTree(IEnumerable<T> source)
        {
            var forest = CreateForest(source);
            if (forest.Count == 1)
            {
                return forest[0];
            }
            if (forest.Count > 1)
            {
                throw new InvalidDataException("Invalid Data Source. There are more than 1 root.");
            }

            throw new InvalidDataException("Invalid Data Source. There is no root.");
        }

        #region Private Methods

        private void AddContentToSelectList(List<SelectListItem> list, HierachyTree<T> node, int level,
            Func<T, int, string> getText, IComparer<T> comparer, object excludeRootId, int deepLitmit)
        {
            if (level > deepLitmit)
            {
                return;
            }
            object id = node.Data.GetId();
            if (Equals(id, excludeRootId))
            {
                return;
            }
            list.Add(new SelectListItem
            {
                Value = id.ToString(),
                Text = getText(node.Data, level)
            });
            if (comparer != null)
            {
                node.OrderChildren(comparer);
            }
            foreach (HierachyTree<T> child in node.Children)
            {
                AddContentToSelectList(list, child, level + 1, getText, comparer, excludeRootId, deepLitmit);
            }
        }

        #endregion
    }
}