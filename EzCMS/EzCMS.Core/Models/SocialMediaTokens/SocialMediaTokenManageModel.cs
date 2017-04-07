using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Utilities.Social.Enums;
using Ez.Framework.IoC;
using EzCMS.Core.Services.SocialMedia;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.SocialMediaTokens
{
    public class SocialMediaTokenManageModel
    {
        #region Constructors

        public SocialMediaTokenManageModel()
        {
            var socialMediaService = HostContainer.GetInstance<ISocialMediaService>();
            SocialMedia = socialMediaService.GetSocialMediaList();
        }

        public SocialMediaTokenManageModel(SocialMediaToken socialMediaToken)
            : this()
        {
            Id = socialMediaToken.Id;
            SocialMediaId = socialMediaToken.SocialMediaId;
            AppId = socialMediaToken.AppId;
            AppSecret = socialMediaToken.AppSecret;
            AccessToken = socialMediaToken.AccessToken;
            AccessTokenSecret = socialMediaToken.AccessTokenSecret;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [RequiredInteger]
        [LocalizedDisplayName("SocialMediaToken_Field_SocialMediaId")]
        public int SocialMediaId { get; set; }

        public IEnumerable<SelectListItem> SocialMedia { get; set; }

        [Required]
        [StringLength(255)]
        [LocalizedDisplayName("SocialMediaToken_Field_AppId")]
        public string AppId { get; set; }

        [Required]
        [StringLength(255)]
        [LocalizedDisplayName("SocialMediaToken_Field_AppSecret")]
        public string AppSecret { get; set; }

        [RequiredIf("SocialMediaId", (int)SocialMediaEnums.SocialNetwork.Twitter)]
        [LocalizedDisplayName("SocialMediaToken_Field_AccessToken")]
        public string AccessToken { get; set; }

        [RequiredIf("SocialMediaId", (int)SocialMediaEnums.SocialNetwork.Twitter)]
        [LocalizedDisplayName("SocialMediaToken_Field_AccessTokenSecret")]
        public string AccessTokenSecret { get; set; }

        #endregion
    }
}
