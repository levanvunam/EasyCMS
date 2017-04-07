using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.NewsNewsCategories
{
    public class NewsNewsCategoryRepository : Repository<NewsNewsCategory>, INewsNewsCategoryRepository
    {
        public NewsNewsCategoryRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Add new reference to categories for news
        /// </summary>
        /// <param name="newsId"></param>
        /// <param name="newsCategoryIds"></param>
        /// <returns></returns>
        public ResponseModel Insert(int newsId, IEnumerable<int> newsCategoryIds)
        {
            return Insert(newsCategoryIds.Select(newsCategoryId => new NewsNewsCategory
            {
                NewsId = newsId,
                NewsCategoryId = newsCategoryId
            }));
        }

        /// <summary>
        /// Delete all reference to categories for news
        /// </summary>
        /// <param name="newsId"></param>
        /// <param name="newsCategoryIds"></param>
        /// <returns></returns>
        public ResponseModel Delete(int newsId, IEnumerable<int> newsCategoryIds)
        {
            var entity =
                FetchFirst(
                    newsNewsCategory =>
                        newsNewsCategory.NewsId == newsId && newsCategoryIds.Contains(newsNewsCategory.NewsCategoryId));
            if (entity != null)
            {
                return Delete(entity);
            }
            return new ResponseModel
            {
                Success = true
            };
        }

        /// <summary>
        /// Delete mapping by news and news category
        /// </summary>
        /// <param name="newsId"></param>
        /// <param name="newsCategoryId"></param>
        /// <returns></returns>
        public ResponseModel Delete(int newsId, int newsCategoryId)
        {
            var entity =
                FetchFirst(
                    newsNewsCategory =>
                        newsNewsCategory.NewsId == newsId && newsNewsCategory.NewsCategoryId == newsCategoryId);
            if (entity != null)
            {
                return Delete(entity);
            }

            return new ResponseModel
            {
                Success = true
            };
        }
    }
}