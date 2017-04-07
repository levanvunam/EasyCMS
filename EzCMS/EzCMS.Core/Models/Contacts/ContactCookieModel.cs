using Ez.Framework.Core.IoC;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Services.Contacts;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Contacts
{
    public class ContactCookieModel
    {
        #region Constructors

        public ContactCookieModel()
        {
        }

        public ContactCookieModel(AnonymousContact anonymousContact)
            : this()
        {
            Email = anonymousContact.Email;
            FirstName = anonymousContact.FirstName;
            LastName = anonymousContact.LastName;
            Phone = anonymousContact.Phone;
            Address = anonymousContact.Address;
            ContactId = anonymousContact.ContactId;
            AnonymousContactId = anonymousContact.Id;
            CookieKey = anonymousContact.CookieKey;
            IpAddress = anonymousContact.IpAddress;
        }

        public ContactCookieModel(User user)
            : this()
        {
            var contactService = HostContainer.GetInstance<IContactService>();
            var contact = contactService.FetchFirst(c => c.UserId == user.Id);
            if (contact != null)
            {
                ContactId = contact.Id;

                Email = contact.Email;
                FirstName = contact.FirstName;
                LastName = contact.LastName;
                Phone = contact.PreferredPhoneNumber ?? contact.PhoneHome ?? contact.PhoneWork ?? contact.MobilePhone;
                Address = contact.AddressLine1 ?? contact.AddressLine2;

                AnonymousContactId = WorkContext.CurrentContact.AnonymousContactId;
                CookieKey = WorkContext.CurrentContact.CookieKey;
                IpAddress = WorkContext.CurrentContact.IpAddress;
            }
            else
            {
                Email = user.Email;
                FirstName = user.FirstName;
                LastName = user.LastName;
                Phone = user.Phone;
                Address = user.Address;

                AnonymousContactId = WorkContext.CurrentContact.AnonymousContactId;
                CookieKey = WorkContext.CurrentContact.CookieKey;
                IpAddress = WorkContext.CurrentContact.IpAddress;
            }
        }

        public ContactCookieModel(Contact contact)
            : this()
        {
            ContactId = contact.Id;

            Email = contact.Email;
            FirstName = contact.FirstName;
            LastName = contact.LastName;
            Phone = contact.PreferredPhoneNumber ?? contact.PhoneHome ?? contact.PhoneWork ?? contact.MobilePhone;
            Address = contact.AddressLine1 ?? contact.AddressLine2;

            AnonymousContactId = WorkContext.CurrentContact.AnonymousContactId;
            CookieKey = WorkContext.CurrentContact.CookieKey;
            IpAddress = WorkContext.CurrentContact.IpAddress;
        }

        #endregion

        #region Public Properties

        public int? ContactId { get; set; }

        public int AnonymousContactId { get; set; }

        public string CookieKey { get; set; }

        public string IpAddress { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        #region Related Fields

        #region Name

        public string FullName
        {
            get
            {
                return StringUtilities.GenerateFullName(FirstName, LastName);
            }
        }

        #endregion

        #region Phone

        public string PreferredPhoneNumber
        {
            get
            {
                return Phone;
            }
        }

        public string MobilePhone
        {
            get
            {
                return Phone;
            }
        }

        public string PhoneWork
        {
            get
            {
                return Phone;
            }
        }

        public string PhoneHome
        {
            get
            {
                return Phone;
            }
        }

        #endregion

        #region Address

        public string AddressLine1
        {
            get
            {
                return Address;
            }
        }

        public string AddressLine2
        {
            get
            {
                return Address;
            }
        }

        #endregion

        #endregion

        #endregion
    }
}