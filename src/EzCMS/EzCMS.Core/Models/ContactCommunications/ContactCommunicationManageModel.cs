using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.Contacts;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.ContactCommunications
{
    public class ContactCommunicationManageModel
    {
        #region Constructors

        private readonly IContactService _contactService;

        public ContactCommunicationManageModel()
        {
            _contactService = HostContainer.GetInstance<IContactService>();
        }

        public ContactCommunicationManageModel(ContactCommunication contactCommunication)
            : this()
        {
            Id = contactCommunication.Id;
            ContactId = contactCommunication.ContactId;
            ContactName = contactCommunication.Contact.FullName;
            ReferredBy = contactCommunication.ReferredBy;
            CampaignCode = contactCommunication.CampaignCode;
            ProductOfInterest = contactCommunication.ProductOfInterest;
            InterestedInOwning = contactCommunication.InterestedInOwning;
            TimeFrameToOwn = contactCommunication.TimeFrameToOwn;
            Certification = contactCommunication.Certification;
            CurrentlyOwn = contactCommunication.CurrentlyOwn;
            PurchaseDate = contactCommunication.PurchaseDate;
            SubscriberType = contactCommunication.SubscriberType;
            Comments = contactCommunication.Comments;
        }

        public ContactCommunicationManageModel(int contactId)
            : this()
        {
            var contact = _contactService.GetById(contactId);
            if (contact != null)
            {
                ContactName = contact.FullName;
                ContactId = contactId;
            }
        }

        #endregion

        #region Public Properties
        public int? Id { get; set; }

        public int ContactId { get; set; }

        [LocalizedDisplayName("ContactCommunication_Field_ContactName")]
        public string ContactName { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("ContactCommunication_Field_ReferredBy")]
        public string ReferredBy { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("ContactCommunication_Field_CampaignCode")]
        public string CampaignCode { get; set; }

        [LocalizedDisplayName("ContactCommunication_Field_Comments")]
        public string Comments { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("ContactCommunication_Field_ProductOfInterest")]
        public string ProductOfInterest { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("ContactCommunication_Field_CurrentlyOwn")]
        public string CurrentlyOwn { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("ContactCommunication_Field_PurchaseDate")]
        public string PurchaseDate { get; set; }

        [LocalizedDisplayName("ContactCommunication_Field_InterestedInOwning")]
        public bool InterestedInOwning { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("ContactCommunication_Field_TimeFrameToOwn")]
        public string TimeFrameToOwn { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("ContactCommunication_Field_Certification")]
        public string Certification { get; set; }

        [StringLength(50)]
        [LocalizedDisplayName("ContactCommunication_Field_SubscriberType")]
        public string SubscriberType { get; set; }

        #endregion
    }
}
