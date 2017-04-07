using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.ContactGroupContacts
{
    public class ContactGroupContactRepository : Repository<ContactGroupContact>, IContactGroupContactRepository
    {
        public ContactGroupContactRepository(EzCMSEntities entities)
            : base(entities)
        {
        }

        /// <summary>
        /// Delete contact groups contact by contact id
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public ResponseModel DeleteByContactId(int contactId)
        {
            var entities = Fetch(m => m.ContactId == contactId);
            return Delete(entities);
        }

        /// <summary>
        /// Delete contact groups contact by contact group id
        /// </summary>
        /// <param name="contactGroupId"></param>
        /// <returns></returns>
        public ResponseModel DeleteByContactGroupId(int contactGroupId)
        {
            var entities = Fetch(m => m.ContactGroupId == contactGroupId);
            return Delete(entities);
        }

        /// <summary>
        /// Insert new contact groups contact for contact
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="contactGroupIds"></param>
        /// <returns></returns>
        public ResponseModel InsertForContact(int contactId, IEnumerable<int> contactGroupIds)
        {
            return Insert(contactGroupIds.Select(id => new ContactGroupContact
            {
                ContactId = contactId,
                ContactGroupId = id
            }));
        }

        /// <summary>
        /// Insert new contact groups contact for contact group
        /// </summary>
        /// <param name="contactGroupId"></param>
        /// <param name="contactIds"></param>
        /// <returns></returns>
        public ResponseModel InsertForGroup(int contactGroupId, IEnumerable<int> contactIds)
        {
            return Insert(contactIds.Select(id => new ContactGroupContact
            {
                ContactId = id,
                ContactGroupId = contactGroupId
            }));
        }
    }
}