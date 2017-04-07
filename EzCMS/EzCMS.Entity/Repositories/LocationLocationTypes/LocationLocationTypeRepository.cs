using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.LocationLocationTypes
{
    public class LocationLocationTypeRepository : Repository<LocationLocationType>, ILocationLocationTypeRepository
    {
        public LocationLocationTypeRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Add mapping by Location and list Type
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="locationTypeIds"></param>
        /// <returns></returns>
        public ResponseModel Insert(int locationId, IEnumerable<int> locationTypeIds)
        {
            return Insert(locationTypeIds.Select(locationTypeId => new LocationLocationType
            {
                LocationId = locationId,
                LocationTypeId = locationTypeId
            }));
        }

        /// <summary>
        /// Add mapping  by location type id and location ids
        /// </summary>
        /// <param name="locationTypeId"></param>
        /// <param name="locationIds"></param>
        /// <returns></returns>
        public ResponseModel InsertByLocationTypeId(int locationTypeId, IEnumerable<int> locationIds)
        {
            return Insert(locationIds.Select(locationId => new LocationLocationType
            {
                LocationId = locationId,
                LocationTypeId = locationTypeId
            }));
        }

        /// <summary>
        /// Delete mapping by Location and list Type
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="locationTypeIds"></param>
        /// <returns></returns>
        public ResponseModel Delete(int locationId, IEnumerable<int> locationTypeIds)
        {
            var entity =
                FetchFirst(
                    locationLocationType =>
                        locationLocationType.LocationId == locationId &&
                        locationTypeIds.Contains(locationLocationType.LocationTypeId));
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
        /// Delete mapping by location type id and location ids
        /// </summary>
        /// <param name="locationTypeId"></param>
        /// <param name="locationIds"></param>
        /// <returns></returns>
        public ResponseModel DeleteByLocationTypeId(int locationTypeId, IEnumerable<int> locationIds)
        {
            var entity =
                FetchFirst(
                    locationLocationType =>
                        locationLocationType.LocationTypeId == locationTypeId &&
                        locationIds.Contains(locationLocationType.LocationId));
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
        /// Delete mapping by location type id and location
        /// </summary>
        /// <param name="locationTypeId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public ResponseModel Delete(int locationTypeId, int locationId)
        {
            var entity =
                FetchFirst(
                    locationLocationType =>
                        locationLocationType.LocationTypeId == locationTypeId &&
                        locationLocationType.LocationId == locationId);
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