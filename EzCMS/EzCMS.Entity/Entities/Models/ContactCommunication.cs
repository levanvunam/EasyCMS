using Ez.Framework.Core.Entity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzCMS.Entity.Entities.Models
{
    public class ContactCommunication : BaseModel
    {
        public int ContactId { get; set; }

        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }

        [StringLength(255)]
        public string ReferredBy { get; set; }

        [StringLength(255)]
        public string CampaignCode { get; set; }

        public string Comments { get; set; }

        public DateTime? CommentDate { get; set; }

        [StringLength(255)]
        public string ProductOfInterest { get; set; }

        [StringLength(255)]
        public string CurrentlyOwn { get; set; }

        [StringLength(255)]
        public string PurchaseDate { get; set; }

        public bool InterestedInOwning { get; set; }

        [StringLength(255)]
        public string TimeFrameToOwn { get; set; }

        [StringLength(255)]
        public string Certification { get; set; }

        [StringLength(50)]
        public string SubscriberType { get; set; }

        [StringLength(255)]
        public string UnsubscribeComment { get; set; }
    }
}