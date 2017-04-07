using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.ProtectedDocumentCompanies
{
    [Register(Lifetime.PerInstance)]
    public interface IProtectedDocumentCompanyRepository : IRepository<ProtectedDocumentCompany>
    {
        ResponseModel Delete(int documentId, List<int> companyIds);

        ResponseModel Insert(int documentId, List<int> companyIds);
    }
}