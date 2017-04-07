using System;
using System.Collections.Generic;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Services.Contacts;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.ContactGroups
{
    public class ContactGroupDetailModel
    {
        public ContactGroupDetailModel()
        {

        }

        public ContactGroupDetailModel(ContactGroup contactGroup)
            : this()
        {
            var contactService = HostContainer.GetInstance<IContactService>();

            Id = contactGroup.Id;

            Name = contactGroup.Name;
            Queries = contactService.GetContactSearchDetailsModels(contactGroup.Queries);

            Created = contactGroup.Created;
            CreatedBy = contactGroup.CreatedBy;
            LastUpdate = contactGroup.LastUpdate;
            LastUpdateBy = contactGroup.LastUpdateBy;
        }

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("ContactGroup_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("ContactGroup_Field_Queries")]
        public IEnumerable<ContactSearchDetailsModel> Queries { get; set; }

        [LocalizedDisplayName("ContactGroup_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("ContactGroup_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("ContactGroup_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("ContactGroup_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        #endregion
    }
}
