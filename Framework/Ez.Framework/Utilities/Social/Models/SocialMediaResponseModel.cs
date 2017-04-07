using System;

namespace Ez.Framework.Utilities.Social.Models
{
    public class SocialMediaResponseModel : SocialMediaAuthorizeModel
    {
        #region Public Properties

        public int? Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Verifier { get; set; }

        public DateTime? ExpiredDate { get; set; }

        #endregion
    }
}
