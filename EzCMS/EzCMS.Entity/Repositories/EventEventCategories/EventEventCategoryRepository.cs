using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.EventEventCategories
{
    public class EventEventCategoryRepository : Repository<EventEventCategory>, IEventEventCategoryRepository
    {
        public EventEventCategoryRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Add mapping by event and list category
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="eventCategoryIds"></param>
        /// <returns></returns>
        public ResponseModel Insert(int eventId, IEnumerable<int> eventCategoryIds)
        {
            return Insert(eventCategoryIds.Select(eventCategoryId => new EventEventCategory
            {
                EventId = eventId,
                EventCategoryId = eventCategoryId
            }));
        }

        /// <summary>
        /// Add mapping by category and list event
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="eventIds"></param>
        /// <returns></returns>
        public ResponseModel InsertByCategoryId(int categoryId, IEnumerable<int> eventIds)
        {
            return Insert(eventIds.Select(eventId => new EventEventCategory
            {
                EventId = eventId,
                EventCategoryId = categoryId
            }));
        }

        /// <summary>
        /// Delete mapping by event and category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public ResponseModel Delete(int categoryId, int eventId)
        {
            var entity =
                FetchFirst(
                    eventEventCategory =>
                        eventEventCategory.EventCategoryId == categoryId && eventEventCategory.EventId == eventId);
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
        /// Delete mapping by event and list category
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="eventCategoryIds"></param>
        /// <returns></returns>
        public ResponseModel Delete(int eventId, IEnumerable<int> eventCategoryIds)
        {
            var entities =
                Fetch(
                    eventEventCategory =>
                        eventEventCategory.EventId == eventId &&
                        eventCategoryIds.Contains(eventEventCategory.EventCategoryId));

            return Delete(entities);
        }

        /// <summary>
        /// Delete mapping  by category and list event
        /// </summary>
        /// <param name="eventCategoryId"></param>
        /// <param name="eventIds"></param>
        /// <returns></returns>
        public ResponseModel DeleteByEventCategoryId(int eventCategoryId, IEnumerable<int> eventIds)
        {
            var entities =
                Fetch(
                    eventEventCategory =>
                        eventEventCategory.EventCategoryId == eventCategoryId &&
                        eventIds.Contains(eventEventCategory.EventId));

            return Delete(entities);
        }
    }
}