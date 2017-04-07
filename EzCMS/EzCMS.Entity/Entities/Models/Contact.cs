using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzCMS.Entity.Entities.Models
{
    public class Contact : BaseModel
    {
        #region Public Properties

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        #region Basic Information

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        public string Company { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(60)]
        public string Department { get; set; }

        #endregion

        public bool IsCompanyAdministrator { get; set; }

        #region Address

        [StringLength(255)]
        public string AddressLine1 { get; set; }

        [StringLength(255)]
        public string AddressLine2 { get; set; }

        [StringLength(100)]
        public string Suburb { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(50)]
        public string Postcode { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        [StringLength(255)]
        public string PostalAddressLine1 { get; set; }

        [StringLength(255)]
        public string PostalAddressLine2 { get; set; }

        [StringLength(100)]
        public string PostalSuburb { get; set; }

        [StringLength(50)]
        public string PostalState { get; set; }

        [StringLength(50)]
        public string PostalPostcode { get; set; }

        [StringLength(100)]
        public string PostalCountry { get; set; }

        #endregion

        #region Phone

        [StringLength(50)]
        public string PreferredPhoneNumber { get; set; }

        [StringLength(50)]
        public string PhoneWork { get; set; }

        [StringLength(50)]
        public string PhoneHome { get; set; }

        [StringLength(50)]
        public string MobilePhone { get; set; }

        [StringLength(50)]
        public string Fax { get; set; }

        #endregion

        #region Communication

        [StringLength(255)]
        public string Occupation { get; set; }

        [StringLength(100)]
        public string Website { get; set; }

        [StringLength(20)]
        public string Sex { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public bool DontSendMarketing { get; set; }

        public bool Unsubscribed { get; set; }

        public DateTime? UnsubscribeDateTime { get; set; }

        public SubscriptionEnums.SubscriptionType SubscriptionType { get; set; }

        public bool? Confirmed { get; set; }

        [StringLength(50)]
        public string ConfirmDateTime { get; set; }

        [StringLength(50)]
        public string FromIPAddress { get; set; }

        public long? ValidatedOk { get; set; }

        public int? ValidateLevel { get; set; }

        [StringLength(255)]
        public string CRMID { get; set; }

        [StringLength(255)]
        public string SalesPerson { get; set; }

        public int? UnsubscribedIssueId { get; set; }

        #endregion

        #region Social

        [StringLength(100)]
        public string Facebook { get; set; }

        [StringLength(100)]
        public string Twitter { get; set; }

        [StringLength(100)]
        public string LinkedIn { get; set; }

        #endregion

        public virtual ICollection<ContactGroupContact> ContactGroupContacts { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; }

        public virtual ICollection<ContactCommunication> ContactCommunications { get; set; }

        public virtual ICollection<AnonymousContact> AnonymousContacts { get; set; }

        #endregion
    }
}