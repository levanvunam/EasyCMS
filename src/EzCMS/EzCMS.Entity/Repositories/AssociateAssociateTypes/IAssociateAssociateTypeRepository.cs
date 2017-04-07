using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.AssociateAssociateTypes
{
    [Register(Lifetime.PerInstance)]
    public interface IAssociateAssociateTypeRepository : IRepository<AssociateAssociateType>
    {
        /// <summary>
        /// Delete by associate id and associate type ids
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="associateTypeIds"></param>
        /// <returns></returns>
        ResponseModel DeleteByAssociateId(int associateId, IEnumerable<int> associateTypeIds);

        /// <summary>
        /// Insert by associate id and associate type ids
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="associateTypeIds"></param>
        /// <returns></returns>
        ResponseModel InsertByAssociateId(int associateId, IEnumerable<int> associateTypeIds);

        /// <summary>
        /// Delete by type id and and associate ids
        /// </summary>
        /// <param name="associateTypeId"></param>
        /// <param name="associateIds"></param>
        /// <returns></returns>
        ResponseModel DeleteByAssociateTypeId(int associateTypeId, IEnumerable<int> associateIds);

        /// <summary>
        /// Create reference for associate type
        /// </summary>
        /// <param name="associateTypeId"></param>
        /// <param name="associateIds"></param>
        /// <returns></returns>
        ResponseModel InsertByAssociateTypeId(int associateTypeId, IEnumerable<int> associateIds);

        /// <summary>
        /// Delete associate associate type
        /// </summary>
        /// <param name="associateTypeId"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        ResponseModel Delete(int associateTypeId, int associateId);
    }
}