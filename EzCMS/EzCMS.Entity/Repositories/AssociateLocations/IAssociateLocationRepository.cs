using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.AssociateLocations
{
    [Register(Lifetime.PerInstance)]
    public interface IAssociateLocationRepository : IRepository<AssociateLocation>
    {
        /// <summary>
        /// Delete by associate id
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="locationIds"></param>
        /// <returns></returns>
        ResponseModel DeleteByAssociateId(int associateId, IEnumerable<int> locationIds);

        /// <summary>
        /// Insert by associate id
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="locationIds"></param>
        /// <returns></returns>
        ResponseModel InsertByAssociateId(int associateId, IEnumerable<int> locationIds);

        /// <summary>
        /// Delete by type id
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="associateIds"></param>
        /// <returns></returns>
        ResponseModel DeleteByLocationId(int locationId, IEnumerable<int> associateIds);

        /// <summary>
        /// Create reference for location
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="associateIds"></param>
        /// <returns></returns>
        ResponseModel InsertByLocationId(int locationId, IEnumerable<int> associateIds);

        /// <summary>
        /// Delete associate location
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        ResponseModel Delete(int locationId, int associateId);
    }
}