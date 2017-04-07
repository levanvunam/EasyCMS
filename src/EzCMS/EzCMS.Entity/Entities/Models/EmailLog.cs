using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Entity.Entities.Models
{
    public class EmailLog : BaseModel
    {
        public EmailEnums.EmailPriority Priority { get; set; }

        [StringLength(500)]
        public string From { get; set; }

        [StringLength(500)]
        public string FromName { get; set; }

        [StringLength(500)]
        public string To { get; set; }

        [StringLength(500)]
        public string ToName { get; set; }

        [StringLength(500)]
        public string CC { get; set; }

        [StringLength(500)]
        public string Bcc { get; set; }

        [StringLength(1000)]
        public string Subject { get; set; }

        public string Body { get; set; }

        public DateTime? SendLater { get; set; }

        public int SentTries { get; set; }

        public DateTime? SentOn { get; set; }

        public string Message { get; set; }

        public string Attachment { get; set; }

        public int EmailAccountId { get; set; }

        [ForeignKey("EmailAccountId")]
        public EmailAccount EmailAccount { get; set; }
    }
}