using System;
using System.Collections.Generic;

namespace EzCMS.Core.Services.Tree
{
    public class Tree<T> : ITree<T>
    {
        public T Data { get; set; }

        public ITree<T> Parent { get; set; }

        public List<ITree<T>> Children { get; set; }

        public ITree<TK> Transform<TK>(Func<T, TK> transformer)
        {
            return Transform((source, lv) => transformer(source));
        }

        public virtual void OrderChildren(IComparer<T> comparer)
        {
            var nodeComparer = new NodeComparer<T>(comparer);
            OrderChildren(this, nodeComparer);
        }

        private static void OrderChildren(ITree<T> node, NodeComparer<T> comparer)
        {
            node.Children.Sort(comparer);
            foreach (var child in node.Children)
            {
                OrderChildren(child, comparer);
            }
        }

        public void RemoveAll(Predicate<T> match)
        {
            RemoveAll(this, match);
        }

        private static void RemoveAll(ITree<T> node, Predicate<T> match)
        {
            node.Children.RemoveAll(i => match(i.Data));
            foreach (var child in node.Children)
            {
                RemoveAll(child, match);
            }
        }

        public ITree<TK> Transform<TK>(Func<T, int, TK> transformer)
        {
            return Transform(transformer, this, 1);
        }

        private static ITree<TK> Transform<TN, TK>(Func<TN, int, TK> transformer, ITree<TN> source, int lv)
        {
            var result = new Tree<TK>
            {
                Data = transformer(source.Data, lv)
            };
            if (source.Children != null)
            {
                result.Children = new List<ITree<TK>>();
                foreach (var child in source.Children)
                {
                    var rchild = Transform(transformer, child, lv + 1);
                    rchild.Parent = result;
                    result.Children.Add(rchild);
                }
            }
            return result;
        }

        private class NodeComparer<TN> : IComparer<ITree<TN>>
        {
            private readonly IComparer<TN> _dataComparer;

            public NodeComparer(IComparer<TN> dataComparer)
            {
                _dataComparer = dataComparer;
            }

            public int Compare(ITree<TN> x, ITree<TN> y)
            {
                return _dataComparer.Compare(x.Data, y.Data);
            }
        }
    }
}