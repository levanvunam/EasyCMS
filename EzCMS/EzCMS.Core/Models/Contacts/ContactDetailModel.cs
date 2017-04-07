using Ez.Framework.Configurations;
using Ez.Framework.Core.Attributes;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Contacts
{
    public class ContactDetailModel
    {
        public ContactDetailModel()
        {

        }

        public ContactDetailModel(Contact contact)
            : this()
        {
            Contact = new ContactManageModel(contact);

            Id = contact.Id;

            UserId = contact.UserId;
            UserFullName = contact.UserId.HasValue ? contact.User.FullName : string.Empty;

            Groups = contact.ContactGroupContacts.Any()
                ? string.Join(FrameworkConstants.ColonSeparator, contact.ContactGroupContacts.Select(g => g.ContactGroup.Name))
                : string.Empty;

            var addressItems = new List<string>();
            if (!string.IsNullOrEmpty(Contact.Suburb)) addressItems.Add(Contact.Suburb);
            if (!string.IsNullOrEmpty(Contact.Postcode)) addressItems.Add(Contact.Postcode);
            if (!string.IsNullOrEmpty(Contact.State)) addressItems.Add(Contact.State);
            if (!string.IsNullOrEmpty(Contact.Country)) addressItems.Add(Contact.Country);

            FullAddress = string.Join(FrameworkConstants.ColonSeparator, addressItems);

            var postalAddressItems = new List<string>();
            if (!string.IsNullOrEmpty(Contact.PostalSuburb)) postalAddressItems.Add(Contact.PostalSuburb);
            if (!string.IsNullOrEmpty(Contact.PostalPostcode)) postalAddressItems.Add(Contact.PostalPostcode);
            if (!string.IsNullOrEmpty(Contact.PostalState)) postalAddressItems.Add(Contact.PostalState);
            if (!string.IsNullOrEmpty(Contact.PostalCountry)) postalAddressItems.Add(Contact.PostalCountry);

            FullPostalAddress = string.Join(FrameworkConstants.ColonSeparator, postalAddressItems);

            Created = contact.Created;
            CreatedBy = contact.CreatedBy;
            LastUpdate = contact.LastUpdate;
            LastUpdateBy = contact.LastUpdateBy;

        }

        #region Public Properties

        [LocalizedDisplayName("Contact_Field_Id")]
        public int Id { get; set; }

        [LocalizedDisplayName("Contact_Field_UserId")]
        public int? UserId { get; set; }

        public string UserFullName { get; set; }

        [LocalizedDisplayName("Contact_Field_GroupNames")]
        public string Groups { get; set; }

        public string FullAddress { get; set; }

        public string FullPostalAddress { get; set; }

        [LocalizedDisplayName("Contact_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("Contact_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("Contact_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("Contact_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public ContactManageModel Contact { get; set; }

        #endregion
    }
}
