using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;

namespace EzCMS.Entity.Repositories.ContactGroupContacts
{
    [Register(Lifetime.PerInstance)]
    public interface IContactGroupContactRepository : IRepository<ContactGroupContact>
    {
        /// <summary>
        /// Delete contact groups contact by contact id
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        ResponseModel DeleteByContactId(int contactId);

        /// <summary>
        /// Delete contact groups contact by contact group id
        /// </summary>
        /// <param name="contactGroupId"></param>
        /// <returns></returns>
        ResponseModel DeleteByContactGroupId(int contactGroupId);

        /// <summary>
        /// Insert new contact groups contact for contact
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="contactGroupIds"></param>
        /// <returns></returns>
        ResponseModel InsertForContact(int contactId, IEnumerable<int> contactGroupIds);

        /// <summary>
        /// Insert new contact groups contact for contact group
        /// </summary>
        /// <param name="contactGroupId"></param>
        /// <param name="contactIds"></param>
        /// <returns></returns>
        ResponseModel InsertForGroup(int contactGroupId, IEnumerable<int> contactIds);
    }
}