using System;
using System.Collections.Generic;

namespace EzCMS.Core.Services.Tree
{
    public interface ITree<T>
    {
        T Data { get; set; }

        ITree<T> Parent { get; set; }

        List<ITree<T>> Children { get; set; }

        ITree<TK> Transform<TK>(Func<T, TK> transformer);
    }
}