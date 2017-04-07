using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.AssociateCompanyTypes
{
    public class AssociateCompanyTypeRepository : Repository<AssociateCompanyType>, IAssociateCompanyTypeRepository
    {
        public AssociateCompanyTypeRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Delete by associate id and company type ids
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="associateTypeIds"></param>
        /// <returns></returns>
        public ResponseModel DeleteByAssociateId(int associateId, IEnumerable<int> associateTypeIds)
        {
            var entities =
                Fetch(
                    associateCompanyType =>
                        associateCompanyType.AssociateId == associateId &&
                        associateTypeIds.Contains(associateCompanyType.CompanyTypeId));
            return Delete(entities);
        }

        /// <summary>
        /// Insert by associate id and company type ids
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="associateTypeIds"></param>
        /// <returns></returns>
        public ResponseModel InsertByAssociateId(int associateId, IEnumerable<int> associateTypeIds)
        {
            return Insert(associateTypeIds.Select(associateTypeId => new AssociateCompanyType
            {
                AssociateId = associateId,
                CompanyTypeId = associateTypeId
            }));
        }


        /// <summary>
        /// Delete by company type id and associate ids
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="associateIds"></param>
        /// <returns></returns>
        public ResponseModel DeleteByCompanyTypeId(int typeId, IEnumerable<int> associateIds)
        {
            var entities =
                Fetch(
                    associateCompanyType =>
                        associateCompanyType.CompanyTypeId == typeId &&
                        associateIds.Contains(associateCompanyType.AssociateId));

            return Delete(entities);
        }

        /// <summary>
        /// Create reference for company type
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="associateIds"></param>
        /// <returns></returns>
        public ResponseModel InsertByCompanyTypeId(int typeId, IEnumerable<int> associateIds)
        {
            return Insert(associateIds.Select(associateId => new AssociateCompanyType
            {
                AssociateId = associateId,
                CompanyTypeId = typeId
            }));
        }

        /// <summary>
        /// Delete associate company type
        /// </summary>
        /// <param name="companyTypeId"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        public ResponseModel Delete(int companyTypeId, int associateId)
        {
            var entity =
                FetchFirst(
                    associateCompanyType =>
                        associateCompanyType.CompanyTypeId == companyTypeId &&
                        associateCompanyType.AssociateId == associateId);
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