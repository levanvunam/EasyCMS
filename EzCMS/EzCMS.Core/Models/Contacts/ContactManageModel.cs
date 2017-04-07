using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Services.Contacts;
using EzCMS.Core.Services.Countries;
using EzCMS.Core.Services.Localizes;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Contacts
{
    public class ContactManageModel : IValidatableObject
    {
        #region Constructors

        public ContactManageModel()
        {
            var countryService = HostContainer.GetInstance<ICountryService>();

            Confirmed = false;
            SexList = EnumUtilities.GenerateSelectListItems<UserEnums.Gender>(GenerateEnumType.StringValueAndDescriptionText);
            Countries = countryService.GetCountries();
        }

        public ContactManageModel(Contact contact)
            : this()
        {
            Id = contact.Id;
            IsCompanyAdministrator = contact.IsCompanyAdministrator;
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
            Occupation = contact.Occupation;
            Website = contact.Website;
            Sex = contact.Sex;
            DateOfBirth = contact.DateOfBirth;
            DontSendMarketing = contact.DontSendMarketing;
            Unsubscribed = contact.Unsubscribed;
            UnsubscribeDateTime = contact.UnsubscribeDateTime;
            SubscriptionType = contact.SubscriptionType;
            Confirmed = contact.Confirmed ?? false;
            ConfirmDateTime = contact.ConfirmDateTime;
            FromIPAddress = contact.FromIPAddress;
            ValidatedOk = contact.ValidatedOk;
            ValidateLevel = contact.ValidateLevel;
            CRMID = contact.CRMID;
            SalesPerson = contact.SalesPerson;
            UnsubscribedIssueId = contact.UnsubscribedIssueId;
            Facebook = contact.Facebook;
            Twitter = contact.Twitter;
            LinkedIn = contact.LinkedIn;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [EmailValidation]
        [StringLength(255)]
        [LocalizedDisplayName("Contact_Field_Email")]
        public string Email { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Contact_Field_Title")]
        public string Title { get; set; }

        [LocalizedDisplayName("Contact_Field_IsCompanyAdministrator")]
        public bool IsCompanyAdministrator { get; set; }

        [LocalizedDisplayName("Contact_Field_FullName")]
        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        [StringLength(100)]
        [LocalizedDisplayName("Contact_Field_FirstName")]
        public string FirstName { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Contact_Field_LastName")]
        public string LastName { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Contact_Field_AddressLine1")]
        public string AddressLine1 { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Contact_Field_AddressLine2")]
        public string AddressLine2 { get; set; }

        [LocalizedDisplayName("Contact_Field_Address")]
        public string Address
        {
            get { return string.Format("{0} or {1}", AddressLine1, AddressLine2); }
        }

        [StringLength(100)]
        [LocalizedDisplayName("Contact_Field_Suburb")]
        public string Suburb { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Contact_Field_State")]
        public string State { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Contact_Field_Postcode")]
        public string Postcode { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Contact_Field_Country")]
        public string Country { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        [StringLength(60)]
        [LocalizedDisplayName("Contact_Field_Department")]
        public string Department { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Contact_Field_PostalAddressLine1")]
        public string PostalAddressLine1 { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Contact_Field_PostalAddressLine2")]
        public string PostalAddressLine2 { get; set; }

        [LocalizedDisplayName("Contact_Field_PostalAddress")]
        public string PostalAddress
        {
            get { return string.Format("{0} or {1}", PostalAddressLine1, PostalAddressLine2); }
        }

        [StringLength(100)]
        [LocalizedDisplayName("Contact_Field_PostalSuburb")]
        public string PostalSuburb { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Contact_Field_PostalState")]
        public string PostalState { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Contact_Field_PostalPostcode")]
        public string PostalPostcode { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Contact_Field_PostalCountry")]
        public string PostalCountry { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Contact_Field_PreferredPhoneNumber")]
        public string PreferredPhoneNumber { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Contact_Field_PhoneWork")]
        public string PhoneWork { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Contact_Field_PhoneHome")]
        public string PhoneHome { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Contact_Field_MobilePhone")]
        public string MobilePhone { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Contact_Field_Fax")]
        public string Fax { get; set; }

        [LocalizedDisplayName("Contact_Field_AgeBracket")]
        public string AgeBracket { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Contact_Field_Company")]
        public string Company { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Contact_Field_Occupation")]
        public string Occupation { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Contact_Field_Website")]
        public string Website { get; set; }

        [StringLength(20)]
        [LocalizedDisplayName("Contact_Field_Gender")]
        public string Sex { get; set; }

        public IEnumerable<SelectListItem> SexList { get; set; }

        [LocalizedDisplayName("Contact_Field_DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [LocalizedDisplayName("Contact_Field_DontSendMarketing")]
        public bool DontSendMarketing { get; set; }

        [LocalizedDisplayName("Contact_Field_Unsubscribed")]
        public bool Unsubscribed { get; set; }

        [LocalizedDisplayName("Contact_Field_UnsubscribeDateTime")]
        public DateTime? UnsubscribeDateTime { get; set; }

        public SubscriptionEnums.SubscriptionType SubscriptionType { get; set; }

        [LocalizedDisplayName("Contact_Field_Confirmed")]
        public bool Confirmed { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Contact_Field_ConfirmDateTime")]
        public string ConfirmDateTime { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("Contact_Field_FromIPAddress")]
        public string FromIPAddress { get; set; }

        [LocalizedDisplayName("Contact_Field_ValidatedOk")]
        public long? ValidatedOk { get; set; }

        [LocalizedDisplayName("Contact_Field_ValidateLevel")]
        public int? ValidateLevel { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Contact_Field_CRMID")]
        public string CRMID { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Contact_Field_SalesPerson")]
        public string SalesPerson { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Contact_Field_Facebook")]
        public string Facebook { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Contact_Field_Twitter")]
        public string Twitter { get; set; }

        [StringLength(100)]
        [LocalizedDisplayName("Contact_Field_LinkedIn")]
        public string LinkedIn { get; set; }

        public int? UnsubscribedIssueId { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var contactService = HostContainer.GetInstance<IContactService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (!Id.HasValue)
            {
                var currentContact = contactService.FetchFirst(c => c.FirstName.Equals(FirstName)
                                && c.LastName.Equals(LastName)
                                && (c.Email.Equals(Email)
                                || c.PhoneHome.Equals(PhoneHome)
                                || c.PhoneWork.Equals(PhoneWork)
                                || c.PreferredPhoneNumber.Equals(PreferredPhoneNumber)));

                if (currentContact != null)
                {
                    var url = UrlUtilities.GenerateUrl(HttpContext.Current.Request.RequestContext, "Contacts", "Details", new
                    {
                        id = currentContact.Id
                    });
                    yield return new ValidationResult(localizedResourceService.TFormat("Contact_Message_ExistsContactInformation", url), new[] { "Contact" });
                }
            }
        }
    }
}