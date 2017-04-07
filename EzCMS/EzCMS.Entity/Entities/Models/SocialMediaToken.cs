using Ez.Framework.Core.Entity.Models;
using Ez.Framework.Utilities.Social.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzCMS.Entity.Entities.Models
{
    public class SocialMediaToken : BaseModel
    {
        public int SocialMediaId { get; set; }

        [ForeignKey("SocialMediaId")]
        public virtual SocialMedia SocialMedia { get; set; }

        public SocialMediaEnums.TokenStatus Status { get; set; }

        [StringLength(255)]
        public string AppId { get; set; }

        [StringLength(255)]
        public string AppSecret { get; set; }

        public bool IsDefault { get; set; }

        [StringLength(255)]
        public string FullName { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }

        public string Verifier { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public virtual ICollection<SocialMediaLog> SocialMediaLogs { get; set; }
    }
}