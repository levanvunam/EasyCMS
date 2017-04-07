using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.AssociateCompanyTypes
{
    [Register(Lifetime.PerInstance)]
    public interface IAssociateCompanyTypeRepository : IRepository<AssociateCompanyType>
    {
        /// <summary>
        /// Delete by associate id and company type ids
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="companyTypeIds"></param>
        /// <returns></returns>
        ResponseModel DeleteByAssociateId(int associateId, IEnumerable<int> companyTypeIds);

        /// <summary>
        /// Delete by company type id and associate ids
        /// </summary>
        /// <param name="companyTypeId"></param>
        /// <param name="associateIds"></param>
        /// <returns></returns>
        ResponseModel DeleteByCompanyTypeId(int companyTypeId, IEnumerable<int> associateIds);

        /// <summary>
        /// Delete associate company type
        /// </summary>
        /// <param name="companyTypeId"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        ResponseModel Delete(int companyTypeId, int associateId);

        /// <summary>
        /// Insert by associate id and company type ids
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="companyTypeIds"></param>
        /// <returns></returns>
        ResponseModel InsertByAssociateId(int associateId, IEnumerable<int> companyTypeIds);

        /// <summary>
        /// Create reference for company type
        /// </summary>
        /// <param name="companyTypeId"></param>
        /// <param name="associateIds"></param>
        /// <returns></returns>
        ResponseModel InsertByCompanyTypeId(int companyTypeId, IEnumerable<int> associateIds);
    }
}