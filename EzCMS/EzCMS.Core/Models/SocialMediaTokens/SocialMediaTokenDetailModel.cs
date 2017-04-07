using Ez.Framework.Core.Attributes;
using Ez.Framework.Utilities.Social.Enums;
using EzCMS.Entity.Entities.Models;
using System;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.SocialMediaTokens
{
    public class SocialMediaTokenDetailModel
    {
        #region Constructors

        public SocialMediaTokenDetailModel()
        {
        }

        public SocialMediaTokenDetailModel(SocialMediaToken socialMediaToken)
            : this()
        {
            Id = socialMediaToken.Id;

            SocialMediaId = socialMediaToken.SocialMediaId;
            SocialMedia = socialMediaToken.SocialMedia.Name;
            IsDefault = socialMediaToken.IsDefault;
            AppId = socialMediaToken.AppId;
            AppSecret = socialMediaToken.AppSecret;
            Status = socialMediaToken.Status;
            ExpiredDate = socialMediaToken.ExpiredDate;
            Verifier = socialMediaToken.Verifier;
            FullName = socialMediaToken.FullName;
            Email = socialMediaToken.Email;

            Created = socialMediaToken.Created;
            CreatedBy = socialMediaToken.CreatedBy;
            LastUpdate = socialMediaToken.LastUpdate;
            LastUpdateBy = socialMediaToken.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_SocialMediaId")]
        public int SocialMediaId { get; set; }

        public string SocialMedia { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_Status")]
        public SocialMediaEnums.TokenStatus Status { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_IsDefault")]
        public bool IsDefault { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_AppId")]
        public string AppId { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_AppSecret")]
        public string AppSecret { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_FullName")]
        public string FullName { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_Email")]
        public string Email { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_AccessToken")]
        public string AccessToken { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_AccessTokenSecret")]
        public string AccessTokenSecret { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_Verifier")]
        public string Verifier { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_ExpiredDate")]
        public DateTime? ExpiredDate { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("SocialMediaToken_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        #endregion
    }
}
