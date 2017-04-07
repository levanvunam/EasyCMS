using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Notice : BaseModel
    {
        [StringLength(512)]
        public string Message { get; set; }

        public bool IsUrgent { get; set; }

        public int NoticeTypeId { get; set; }

        [ForeignKey("NoticeTypeId")]
        public NoticeType NoticeType { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }
    }
}