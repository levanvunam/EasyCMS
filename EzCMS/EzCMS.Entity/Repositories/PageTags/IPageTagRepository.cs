using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.PageTags
{
    [Register(Lifetime.PerInstance)]
    public interface IPageTagRepository : IRepository<PageTag>
    {
        /// <summary>
        /// Delete page tags
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="tagIds"></param>
        /// mmmm
        /// <returns></returns>
        ResponseModel Delete(int pageId, List<int> tagIds);

        /// <summary>
        /// Delete page tags
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="tags"></param>
        /// mmmm
        /// <returns></returns>
        ResponseModel Delete(int pageId, List<string> tags);

        /// <summary>
        /// Create page tags
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="tagIds"></param>
        /// <returns></returns>
        ResponseModel Insert(int pageId, List<int> tagIds);

        /// <summary>
        /// Create page tags
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        ResponseModel Insert(int pageId, List<string> tags);

        /// <summary>
        /// Save page tags
        /// </summary>
        /// <param name="page"> </param>
        /// <param name="tagString"></param>
        /// <returns></returns>
        ResponseModel SavePageTags(Page page, string tagString);
    }
}