using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;
using System.Linq;

namespace EzCMS.Entity.Repositories.AssociateAssociateTypes
{
    public class AssociateAssociateTypeRepository : Repository<AssociateAssociateType>,
        IAssociateAssociateTypeRepository
    {
        public AssociateAssociateTypeRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Delete mapping by Associate and list Type
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="associateTypeIds"></param>
        /// <returns></returns>
        public ResponseModel DeleteByAssociateId(int associateId, IEnumerable<int> associateTypeIds)
        {
            var entities =
                Fetch(
                    associateAssociateType =>
                        associateAssociateType.AssociateId == associateId &&
                        associateTypeIds.Contains(associateAssociateType.AssociateTypeId));
            return Delete(entities);
        }

        /// <summary>
        /// Add mapping by Associate and list Type
        /// </summary>
        /// <param name="associateId"></param>
        /// <param name="associateTypeIds"></param>
        /// <returns></returns>
        public ResponseModel InsertByAssociateId(int associateId, IEnumerable<int> associateTypeIds)
        {
            return Insert(associateTypeIds.Select(associateTypeId => new AssociateAssociateType
            {
                AssociateId = associateId,
                AssociateTypeId = associateTypeId
            }));
        }

        /// <summary>
        /// Delete mapping by associate type id and associate ids
        /// </summary>
        /// <param name="associateTypeId"></param>
        /// <param name="associateIds"></param>
        /// <returns></returns>
        public ResponseModel DeleteByAssociateTypeId(int associateTypeId, IEnumerable<int> associateIds)
        {
            var entities = Fetch(associateAssociateType =>
                associateAssociateType.AssociateTypeId == associateTypeId &&
                associateIds.Contains(associateAssociateType.AssociateId));

            return Delete(entities);
        }

        /// <summary>
        /// Add mapping by Type and list Associate
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="associateIds"></param>
        /// <returns></returns>
        public ResponseModel InsertByAssociateTypeId(int typeId, IEnumerable<int> associateIds)
        {
            return Insert(associateIds.Select(associateId => new AssociateAssociateType
            {
                AssociateId = associateId,
                AssociateTypeId = typeId
            }));
        }

        /// <summary>
        /// Delete mapping by Associate and Type
        /// </summary>
        /// <param name="associateTypeId"></param>
        /// <param name="associateId"></param>
        /// <returns></returns>
        public ResponseModel Delete(int associateTypeId, int associateId)
        {
            var entity =
                FetchFirst(
                    associateAssociateType =>
                        associateAssociateType.AssociateTypeId == associateTypeId &&
                        associateAssociateType.AssociateId == associateId);
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