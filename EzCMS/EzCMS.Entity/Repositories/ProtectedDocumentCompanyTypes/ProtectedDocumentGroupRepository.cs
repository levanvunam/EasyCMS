using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.ProtectedDocumentCompanyTypes
{
    public class ProtectedDocumentCompanyTypeRepository : Repository<ProtectedDocumentCompanyType>,
        IProtectedDocumentCompanyTypeRepository
    {
        public ProtectedDocumentCompanyTypeRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Delete protected document company types
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="companyTypeIds"></param>
        /// <returns></returns>
        public ResponseModel Delete(int documentId, List<int> companyTypeIds)
        {
            var entity = FetchFirst(t => t.ProtectedDocumentId == documentId && companyTypeIds.Contains(t.CompanyTypeId));
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
        /// Add new protected document company types
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="companyTypeIds"></param>
        /// <returns></returns>
        public ResponseModel Insert(int documentId, List<int> companyTypeIds)
        {
            return Insert(companyTypeIds.Select(id => new ProtectedDocumentCompanyType
            {
                ProtectedDocumentId = documentId,
                CompanyTypeId = id
            }));
        }
    }
}