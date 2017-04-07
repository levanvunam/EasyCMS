using System;
using System.Linq;
using System.Linq.Expressions;

namespace Ez.Framework.Core.Services
{
    public interface IBaseService<TModel> where TModel : class
    {

        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        IQueryable<TModel> GetAll();

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IQueryable<TModel> Fetch(Expression<Func<TModel, bool>> expression);

        /// <summary>
        /// Get first
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        TModel FetchFirst(Expression<Func<TModel, bool>> expression);

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TModel GetById(object id);
    }
}
