using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Services.LinkTrackers;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.Pages;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.LinkTrackers
{
    public class LinkTrackerManageModel : IValidatableObject
    {
        #region Constructors

        public LinkTrackerManageModel()
        {
            var pageService = HostContainer.GetInstance<IPageService>();

            Pages = pageService.GetAccessablePageSelectList();
            LinkTrackerType = LinkTrackerEnums.LinkTrackerType.Internal;
            LinkTrackerTypes = EnumUtilities.GetAllItems<LinkTrackerEnums.LinkTrackerType>();
        }

        public LinkTrackerManageModel(LinkTracker linkTracker)
            : this()
        {
            Id = linkTracker.Id;

            Name = linkTracker.Name;
            IsAllowMultipleClick = linkTracker.IsAllowMultipleClick;
            LinkTrackerType = linkTracker.PageId.HasValue ? LinkTrackerEnums.LinkTrackerType.Internal : LinkTrackerEnums.LinkTrackerType.External;
            RedirectUrl = linkTracker.RedirectUrl;
            PageId = linkTracker.PageId;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("LinkTracker_Field_Name")]
        public string Name { get; set; }

        [Required]
        [LocalizedDisplayName("LinkTracker_Field_LinkTrackerType")]
        public LinkTrackerEnums.LinkTrackerType LinkTrackerType { get; set; }

        public IEnumerable<LinkTrackerEnums.LinkTrackerType> LinkTrackerTypes { get; set; }

        [Required]
        [LocalizedDisplayName("LinkTracker_Field_IsAllowMultipleClick")]
        public bool IsAllowMultipleClick { get; set; }

        [Url]
        [RequiredIf("PageId", null)]
        [LocalizedDisplayName("LinkTracker_Field_RedirectUrl")]
        public string RedirectUrl { get; set; }

        [RequiredIf("RedirectUrl", null)]
        [LocalizedDisplayName("LinkTracker_Field_PageId")]
        public int? PageId { get; set; }

        public IEnumerable<SelectListItem> Pages { get; set; }

        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var linkTrackerService = HostContainer.GetInstance<ILinkTrackerService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (linkTrackerService.IsNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("LinkTracker_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}
