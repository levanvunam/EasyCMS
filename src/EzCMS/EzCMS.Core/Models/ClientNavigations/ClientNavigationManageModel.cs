using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Enums;
using Ez.Framework.Core.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Services.ClientNavigations;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.ClientNavigations
{
    public class ClientNavigationManageModel
    {
        private readonly IClientNavigationService _clientNavigationService;

        public ClientNavigationManageModel()
        {
            _clientNavigationService = HostContainer.GetInstance<IClientNavigationService>();
            IncludeInSiteNavigation = true;
            DisableNavigationCascade = false;
            Parents = _clientNavigationService.GetPossibleParents();

            int position;
            int relativePageId;
            var relativePages = _clientNavigationService.GetRelativeNavigations(out position, out relativePageId);
            Positions = EnumUtilities.GenerateSelectListItems<PageEnums.PagePosition>();
            UrlTargets = EnumUtilities.GenerateSelectListItems<CommonEnums.UrlTarget>(GenerateEnumType.DescriptionValueAndDescriptionText);
            Position = position;
            RelativeNavigationId = relativePageId;
            RelativeNavigations = relativePages;
        }

        public ClientNavigationManageModel(ClientNavigation navigation)
            : this()
        {
            Id = navigation.Id;
            Title = navigation.Title;
            Url = navigation.Url;
            UrlTarget = navigation.UrlTarget;
            IncludeInSiteNavigation = navigation.IncludeInSiteNavigation;
            DisableNavigationCascade = navigation.DisableNavigationCascade;
            StartPublishingDate = navigation.StartPublishingDate;
            EndPublishingDate = navigation.EndPublishingDate;
            ParentId = navigation.ParentId;
            Parents = _clientNavigationService.GetPossibleParents(navigation.Id);

            int position;
            int relativePageId;
            var relativePages = _clientNavigationService.GetRelativeNavigations(out position, out relativePageId, navigation.Id, navigation.ParentId);
            Position = position;
            RelativeNavigationId = relativePageId;
            RelativeNavigations = relativePages;
        }

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("ClientNavigation_Field_Title")]
        public string Title { get; set; }

        [Required]
        [LocalizedDisplayName("ClientNavigation_Field_Url")]
        public string Url { get; set; }

        [LocalizedDisplayName("ClientNavigation_Field_UrlTarget")]
        public string UrlTarget { get; set; }

        public IEnumerable<SelectListItem> UrlTargets { get; set; }

        [LocalizedDisplayName("ClientNavigation_Field_IncludeInSiteNavigation")]
        public bool IncludeInSiteNavigation { get; set; }

        [LocalizedDisplayName("ClientNavigation_Field_DisableNavigationCascade")]
        public bool DisableNavigationCascade { get; set; }

        [LocalizedDisplayName("ClientNavigation_Field_StartPublishingDate")]
        public DateTime? StartPublishingDate { get; set; }

        [LocalizedDisplayName("ClientNavigation_Field_EndPublishingDate")]
        public DateTime? EndPublishingDate { get; set; }

        [LocalizedDisplayName("ClientNavigation_Field_ParentNavigationDropdown")]
        public int? ParentId { get; set; }

        public IEnumerable<SelectListItem> Parents { get; set; }

        [LocalizedDisplayName("ClientNavigation_Field_Position")]
        public int Position { get; set; }

        public IEnumerable<SelectListItem> Positions { get; set; }

        [LocalizedDisplayName("ClientNavigation_Field_RelativeNavigations")]
        public int? RelativeNavigationId { get; set; }

        public IEnumerable<SelectListItem> RelativeNavigations { get; set; }

        #endregion
    }
}
