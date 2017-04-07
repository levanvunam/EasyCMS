using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.EventEventCategories
{
    [Register(Lifetime.PerInstance)]
    public interface IEventEventCategoryRepository : IRepository<EventEventCategory>
    {
        ResponseModel Delete(int eventId, IEnumerable<int> eventCategoryIds);

        ResponseModel Insert(int eventId, IEnumerable<int> eventCategoryIds);

        ResponseModel DeleteByEventCategoryId(int eventCategoryId, IEnumerable<int> eventIds);

        ResponseModel InsertByCategoryId(int categoryId, IEnumerable<int> eventIds);

        ResponseModel Delete(int categoryId, int eventId);
    }
}