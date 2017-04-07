using Ez.Framework.Configurations;
using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EzCMS.Core.Models.Contacts
{
    public class ContactModel : BaseModel
    {
        #region Constructors
        public ContactModel()
        {

        }

        public ContactModel(Contact contact)
            : this()
        {
            Id = contact.Id;
            BelongToUser = contact.UserId.HasValue;
            UserId = contact.UserId;
            Email = contact.Email;
            Title = contact.Title;
            FirstName = contact.FirstName;
            LastName = contact.LastName;
            AddressLine1 = contact.AddressLine1;
            AddressLine2 = contact.AddressLine2;
            Suburb = contact.Suburb;
            State = contact.State;
            Postcode = contact.Postcode;
            Country = contact.Country;
            Department = contact.Department;
            PostalAddressLine1 = contact.PostalAddressLine1;
            PostalAddressLine2 = contact.PostalAddressLine2;
            PostalSuburb = contact.PostalSuburb;
            PostalState = contact.PostalState;
            PostalPostcode = contact.PostalPostcode;
            PostalCountry = contact.PostalCountry;
            PreferredPhoneNumber = contact.PreferredPhoneNumber;
            PhoneWork = contact.PhoneWork;
            PhoneHome = contact.PhoneHome;
            MobilePhone = contact.MobilePhone;
            Fax = contact.Fax;
            Company = contact.Company;
            GroupNames = contact.ContactGroupContacts.Select(x => x.ContactGroup.Name);
            Occupation = contact.Occupation;
            Website = contact.Website;
            Sex = contact.Sex;
            DateOfBirth = contact.DateOfBirth;
            DontSendMarketing = contact.DontSendMarketing;
            Unsubscribed = contact.Unsubscribed;
            UnsubscribeDateTime = contact.UnsubscribeDateTime;
            SubscriptionType = contact.SubscriptionType;
            Confirmed = contact.Confirmed;
            ConfirmDateTime = contact.ConfirmDateTime;
            FromIPAddress = contact.FromIPAddress;
            ValidatedOk = contact.ValidatedOk;
            ValidateLevel = contact.ValidateLevel;
            CRMID = contact.CRMID;
            SalesPerson = contact.SalesPerson;
            UnsubscribedIssueId = contact.UnsubscribedIssueId;
            Created = contact.Created;
            CreatedBy = contact.CreatedBy;
            LastUpdate = contact.LastUpdate;
            LastUpdateBy = contact.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public bool BelongToUser { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        #region Basic Information

        public string Email { get; set; }

        public string Title { get; set; }

        public string Company { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Department { get; set; }

        #endregion

        #region Address

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string Suburb { get; set; }

        public string State { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }

        public string PostalAddressLine1 { get; set; }

        public string PostalAddressLine2 { get; set; }

        public string PostalSuburb { get; set; }

        public string PostalState { get; set; }

        public string PostalPostcode { get; set; }

        public string PostalCountry { get; set; }

        #endregion

        #region Phone

        public string PreferredPhoneNumber { get; set; }

        public string PhoneWork { get; set; }

        public string PhoneHome { get; set; }

        public string MobilePhone { get; set; }

        public string Fax { get; set; }

        #endregion

        #region Communication

        public string Occupation { get; set; }

        public string Website { get; set; }

        public string Sex { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public bool DontSendMarketing { get; set; }

        public bool Unsubscribed { get; set; }

        public DateTime? UnsubscribeDateTime { get; set; }

        public SubscriptionEnums.SubscriptionType SubscriptionType { get; set; }

        public bool? Confirmed { get; set; }

        public string ConfirmDateTime { get; set; }

        public string FromIPAddress { get; set; }

        public long? ValidatedOk { get; set; }

        public int? ValidateLevel { get; set; }

        public string CRMID { get; set; }

        public string SalesPerson { get; set; }

        public int? UnsubscribedIssueId { get; set; }

        #endregion

        #region Social

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        public string LinkedIn { get; set; }

        #endregion

        public bool IsCompanyAdministrator { get; set; }

        public string Groups
        {
            get
            {
                return string.Join(FrameworkConstants.SemicolonSeparator, GroupNames);
            }
        }

        public IEnumerable<string> GroupNames { get; set; }

        public int? UserId { get; set; }

        #endregion
    }
}
