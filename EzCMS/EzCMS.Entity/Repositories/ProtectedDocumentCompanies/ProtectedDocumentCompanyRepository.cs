using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.ProtectedDocumentCompanies
{
    public class ProtectedDocumentCompanyRepository : Repository<ProtectedDocumentCompany>,
        IProtectedDocumentCompanyRepository
    {
        public ProtectedDocumentCompanyRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Delete protected document companies
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="companyIds"></param>
        /// <returns></returns>
        public ResponseModel Delete(int documentId, List<int> companyIds)
        {
            var entity =
                FetchFirst(
                    protectedDocumentCompany =>
                        protectedDocumentCompany.ProtectedDocumentId == documentId &&
                        companyIds.Contains(protectedDocumentCompany.CompanyId));
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
        /// Add new protected document companies
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="companyIds"></param>
        /// <returns></returns>
        public ResponseModel Insert(int documentId, List<int> companyIds)
        {
            return Insert(companyIds.Select(id => new ProtectedDocumentCompany
            {
                ProtectedDocumentId = documentId,
                CompanyId = id
            }));
        }
    }
}