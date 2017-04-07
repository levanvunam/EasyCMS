using Ez.Framework.Core.IoC;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Social.Enums;
using Ez.Framework.IoC;
using EzCMS.Core.Services.SocialMediaTokens;

namespace EzCMS.Core.Models.SocialMedia.Feed
{
    public class SocialMediaMessageModel
    {
        #region Constructors

        public SocialMediaMessageModel()
        {

        }

        public SocialMediaMessageModel(Entity.Entities.Models.SocialMedia socialMedia)
            : this()
        {
            var socialMediaTokenService = HostContainer.GetInstance<ISocialMediaTokenService>();

            SocialMediaId = socialMedia.Id;
            SocialNetwork = socialMedia.Name.ToEnum<SocialMediaEnums.SocialNetwork>();
            PostStatus = false;
            Message = string.Empty;

            var token = socialMediaTokenService.GetActiveTokenOfSocialMedia(SocialMediaId);

            if (token != null)
            {
                FullName = token.FullName;
                Message = token.Email;
                SocialMediaTokenId = token.Id;
            }
        }

        #endregion

        #region Public Properties

        public int SocialMediaId { get; set; }

        public SocialMediaEnums.SocialNetwork SocialNetwork { get; set; }

        public bool IsSetup
        {
            get
            {
                return SocialMediaTokenId.HasValue;
            }
        }

        public int? SocialMediaTokenId { get; set; }

        public bool PostStatus { get; set; }

        public string Message { get; set; }

        public string PageTitle { get; set; }

        #region Token information

        public string Email { get; set; }

        public string FullName { get; set; }

        #endregion

        #endregion
    }
}
