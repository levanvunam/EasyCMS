using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Models;
using EzCMS.Entity.Entities.Models;
using System;

namespace EzCMS.Core.Models.ContactCommunications
{
    public class ContactCommunicationModel : BaseGridModel
    {
        public ContactCommunicationModel()
        {

        }

        public ContactCommunicationModel(ContactCommunication contactCommunication)
            : this()
        {
            Id = contactCommunication.Id;
            CurrentlyOwn = contactCommunication.CurrentlyOwn;
            InterestedInOwning = contactCommunication.InterestedInOwning;
            ProductOfInterest = contactCommunication.ProductOfInterest;
            PurchaseDate = contactCommunication.PurchaseDate;
            CampaignCode = contactCommunication.CampaignCode;
            Certification = contactCommunication.Certification;
            SubscriberType = contactCommunication.SubscriberType;
            ReferredBy = contactCommunication.ReferredBy;
            TimeFrameToOwn = contactCommunication.TimeFrameToOwn;

            RecordOrder = contactCommunication.RecordOrder;
            Created = contactCommunication.Created;
            CreatedBy = contactCommunication.CreatedBy;
            LastUpdate = contactCommunication.LastUpdate;
            LastUpdateBy = contactCommunication.LastUpdateBy;
        }

        #region Public Properties

        public string ReferredBy { get; set; }

        public string CampaignCode { get; set; }

        public string ProductOfInterest { get; set; }

        public bool InterestedInOwning { get; set; }

        public string TimeFrameToOwn { get; set; }

        public string Certification { get; set; }

        public string CurrentlyOwn { get; set; }

        public string PurchaseDate { get; set; }

        public string SubscriberType { get; set; }

        [DefaultOrder(Order = GridSort.Desc)]
        public new DateTime Created { get; set; }

        #endregion
    }
}
