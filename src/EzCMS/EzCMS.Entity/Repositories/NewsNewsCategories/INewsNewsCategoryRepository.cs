using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;

namespace EzCMS.Entity.Repositories.NewsNewsCategories
{
    [Register(Lifetime.PerInstance)]
    public interface INewsNewsCategoryRepository : IRepository<NewsNewsCategory>
    {
        /// <summary>
        /// Insert mapping by news
        /// </summary>
        /// <param name="newsId"></param>
        /// <param name="newsCategoryIds"></param>
        /// <returns></returns>
        ResponseModel Insert(int newsId, IEnumerable<int> newsCategoryIds);

        /// <summary>
        /// Delete mapping between news and news category
        /// </summary>
        /// <param name="newsId"></param>
        /// <param name="newsCategoryId"></param>
        /// <returns></returns>
        ResponseModel Delete(int newsId, int newsCategoryId);

        /// <summary>
        /// Delete mapping for news
        /// </summary>
        /// <param name="newsId"></param>
        /// <param name="newsCategoryIds"></param>
        /// <returns></returns>
        ResponseModel Delete(int newsId, IEnumerable<int> newsCategoryIds);
    }
}