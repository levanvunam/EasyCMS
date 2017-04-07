using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.AssociateLocations
{
    public class AssociateLocationRepository : Repository<AssociateLocation>, IAssociateLocationRepository
    {
        public AssociateLocationRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Delete mapping by Associate and list Type
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="locationIds"></param>
        /// <returns></returns>
        public ResponseModel DeleteByAssociateId(int associateId, IEnumerable<int> locationIds)
        {
            var entities =
                Fetch(
                    associateLocation =>
                        associateLocation.AssociateId == associateId &&
                        locationIds.Contains(associateLocation.LocationId));

            return Delete(entities);
        }

        /// <summary>
        /// Add mapping by Associate and list Type
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="locationIds"></param>
        /// <returns></returns>
        public ResponseModel InsertByAssociateId(int associateId, IEnumerable<int> locationIds)
        {
            return Insert(locationIds.Select(locationId => new AssociateLocation
            {
                AssociateId = associateId,
                LocationId = locationId
            }));
        }


        /// <summary>
        /// Delete mapping  by Type and list Associate
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="associateIds"></param>
        /// <returns></returns>
        public ResponseModel DeleteByLocationId(int typeId, IEnumerable<int> associateIds)
        {
            var entities =
                Fetch(
                    associateLocation =>
                        associateLocation.LocationId == typeId && associateIds.Contains(associateLocation.AssociateId));

            return Delete(entities);
        }

        /// <summary>
        /// Add mapping  by Type and list Associate
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="associateIds"></param>
        /// <returns></returns>
        public ResponseModel InsertByLocationId(int typeId, IEnumerable<int> associateIds)
        {
            return Insert(associateIds.Select(associateId => new AssociateLocation
            {
                AssociateId = associateId,
                LocationId = typeId
            }));
        }

        /// <summary>
        /// Delete mapping by Associate and Type
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        public ResponseModel Delete(int locationId, int associateId)
        {
            var entity =
                FetchFirst(
                    associateLocation =>
                        associateLocation.LocationId == locationId && associateLocation.AssociateId == associateId);
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