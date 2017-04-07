using Ez.Framework.Models;
using Ez.Framework.Utilities.Social.Enums;
using System;

namespace EzCMS.Core.Models.SocialMediaTokens
{
    public class SocialMediaTokenModel : BaseGridModel
    {
        #region Public Properties

        public int SocialMediaId { get; set; }

        public string SocialMedia { get; set; }

        public SocialMediaEnums.TokenStatus Status { get; set; }

        public bool IsDefault { get; set; }

        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }

        public string Verifier { get; set; }

        public DateTime? ExpiredDate { get; set; }

        #endregion
    }
}
