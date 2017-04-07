using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.SocialMedia;
using EzCMS.Core.Services.SocialMediaTokens;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.SocialMedia.WidgetManageModels
{
    public class SocialFeedWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public SocialFeedWidgetManageModel()
        {
        }

        public SocialFeedWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var socialMediaTokenService = HostContainer.GetInstance<ISocialMediaTokenService>();
            var socialMediaService = HostContainer.GetInstance<ISocialMediaService>();

            Tokens = socialMediaTokenService.GetActiveTokens();
            SocialMediaList = socialMediaService.GetSocialMediaList();

            ParseParams(parameters);
        }

        #endregion

        #region Parse Params

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * TokenId
             * * Total
             * * Template 
             */

            //TokenId
            if (parameters.Length > 1)
            {
                TokenId = parameters[1].ToInt(0);
            }

            //Total
            if (parameters.Length > 2)
            {
                Total = parameters[2].ToInt(0);
            }

            //Template 
            if (parameters.Length > 3)
            {
                Template = parameters[3];
            }
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("Widget_SocialFeed_Field_SocialMediaId")]
        public int SocialMediaId { get; set; }

        public IEnumerable<SelectListItem> SocialMediaList { get; set; }

        [LocalizedDisplayName("Widget_SocialFeed_Field_TokenId")]
        public int TokenId { get; set; }

        public IEnumerable<SelectListItem> Tokens { get; set; }

        [LocalizedDisplayName("Widget_SocialFeed_Field_Total")]
        public int Total { get; set; }

        #endregion
    }
}
