using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.RssFeeds;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.RssFeeds
{
    public class RssFeedManageModel : IValidatableObject
    {
        public RssFeedManageModel()
        {
            RssFeedTypes = EnumUtilities.GenerateSelectListItems<RssFeedEnums.RssType>(GenerateEnumType.IntValueAndDescriptionText).ToList();
        }

        public RssFeedManageModel(RssFeed rssFeed) : this()
        {
            Id = rssFeed.Id;
            Name = rssFeed.Name;
            Url = rssFeed.Url;
            RssType = rssFeed.RssType;

            if (RssType == RssFeedEnums.RssType.GoogleBlogger)
            {
                string[] parts;
                if (Url.TryParseExact(EzCMSContants.GoogleBlogRssUrlFormat, out parts, true))
                {
                    if (parts.Any())
                    {
                        BlogName = parts[0];
                    }
                }
            }
        }

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("RSSFeed_Field_Name")]
        public string Name { get; set; }

        [Required]
        [LocalizedDisplayName("RSSFeed_Field_Url")]
        public string Url { get; set; }

        [RequiredIf("RSSType", (int)RssFeedEnums.RssType.GoogleBlogger)]
        [LocalizedDisplayName("RSSFeed_Field_BlogName")]
        public string BlogName { get; set; }

        [LocalizedDisplayName("RSSFeed_Field_RSSType")]
        public RssFeedEnums.RssType RssType { get; set; }

        public List<SelectListItem> RssFeedTypes { get; set; }

        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var rssFeedService = HostContainer.GetInstance<IRssFeedService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (rssFeedService.IsNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("RSSFeed_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}
