using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.RssFeeds;
using EzCMS.Entity.Core.Enums;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.RssFeeds.WidgetManageModels
{
    public class RssFeedWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public RssFeedWidgetManageModel()
        {

        }

        public RssFeedWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var rssFeedService = HostContainer.GetInstance<IRssFeedService>();

            RssFeeds = rssFeedService.GetRssFeeds(RssType);
            RssFeedTypes = EnumUtilities.GenerateSelectListItems<RssFeedEnums.RssType>(GenerateEnumType.IntValueAndDescriptionText);

            ParseParams(parameters);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parse params
        /// </summary>
        /// <param name="parameters"></param>
        private void ParseParams(string[] parameters)
        {
            //RssFeedId
            if (parameters.Length > 1)
            {
                RssFeedId = parameters[1].ToInt();
            }

            //Number
            if (parameters.Length > 2)
            {
                Number = parameters[2].ToInt();
            }

            //Template 
            if (parameters.Length > 3)
            {
                Template = parameters[3];
            }
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("Widget_RSSFeed_Field_RSSFeedId")]
        public int RssFeedId { get; set; }

        [LocalizedDisplayName("Widget_RSSFeed_Field_RSSFeedType")]
        public RssFeedEnums.RssType? RssType { get; set; }

        [LocalizedDisplayName("Widget_RSSFeed_Field_Number")]
        public int? Number { get; set; }

        public IEnumerable<SelectListItem> RssFeedTypes { get; set; } 

        public IEnumerable<SelectListItem> RssFeeds { get; set; }

        #endregion
    }
}
