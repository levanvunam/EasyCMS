using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;

namespace EzCMS.Entity.Repositories.LocationLocationTypes
{
    [Register(Lifetime.PerInstance)]
    public interface ILocationLocationTypeRepository : IRepository<LocationLocationType>
    {
        /// <summary>
        /// Insert by location id and location type ids
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="locationTypeIds"></param>
        /// <returns></returns>
        ResponseModel Insert(int locationId, IEnumerable<int> locationTypeIds);

        /// <summary>
        /// Create reference for location type
        /// </summary>
        /// <param name="locationTypeId"></param>
        /// <param name="locationIds"></param>
        /// <returns></returns>
        ResponseModel InsertByLocationTypeId(int locationTypeId, IEnumerable<int> locationIds);

        /// <summary>
        /// Delete location location type
        /// </summary>
        /// <param name="locationTypeId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        ResponseModel Delete(int locationTypeId, int locationId);

        /// <summary>
        /// Delete by type id and and location ids
        /// </summary>
        /// <param name="locationTypeId"></param>
        /// <param name="locationIds"></param>
        /// <returns></returns>
        ResponseModel DeleteByLocationTypeId(int locationTypeId, IEnumerable<int> locationIds);

        /// <summary>
        /// Delete by location id and location type ids
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="locationTypeIds"></param>
        /// <returns></returns>
        ResponseModel Delete(int locationId, IEnumerable<int> locationTypeIds);
    }
}