using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Files;
using Ez.Framework.Utilities.Web;
using Ez.Framework.IoC;
using EzCMS.Core.Framework.Utilities;
using EzCMS.Core.Models.Notifications.ModuleParameters;
using EzCMS.Core.Models.Notifications.Setup;
using EzCMS.Core.Models.SocialMedia.Feed;
using EzCMS.Core.Models.SubscriptionLogs;
using EzCMS.Core.Models.Subscriptions.ModuleParameters;
using EzCMS.Core.Services.BodyTemplates;
using EzCMS.Core.Services.FileTemplates;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.Pages;
using EzCMS.Core.Services.PageTemplates;
using EzCMS.Core.Services.SocialMediaTokens;
using EzCMS.Core.Services.Styles;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Pages
{
    public class PageManageModel : IValidatableObject
    {
        private readonly IPageService _pageService;
        private readonly IBodyTemplateService _bodyTemplateService;
        private readonly IPageTemplateService _pageTemplateService;
        private readonly IFileTemplateService _fileTemplateService;
        public const string SocialMessageFormat = "{0} {1}";

        #region Constructors

        public PageManageModel()
        {
            _pageService = HostContainer.GetInstance<IPageService>();
            _bodyTemplateService = HostContainer.GetInstance<IBodyTemplateService>();
            _pageTemplateService = HostContainer.GetInstance<IPageTemplateService>();
            _fileTemplateService = HostContainer.GetInstance<IFileTemplateService>();
            var styleService = HostContainer.GetInstance<IStyleService>();
            var socialMediaTokenService = HostContainer.GetInstance<ISocialMediaTokenService>();

            int position;
            int relativePageId;
            var relativePages = _pageService.GetRelativePages(out position, out relativePageId);

            StatusList = _pageService.GetStatus();
            Parents = _pageService.GetPossibleParents();
            PageTemplates = _pageTemplateService.GetPageTemplateSelectList();
            FileTemplates = _fileTemplateService.GetFileTemplates();
            Positions = EnumUtilities.GenerateSelectListItems<PageEnums.PagePosition>();
            PageSecurityModels = _pageService.GetPageSecurities();
            BodyTemplates = _bodyTemplateService.GetBodyTemplates();
            IncludedStyles = styleService.GetIncludedStyles();

            Position = position;
            RelativePageId = relativePageId;
            RelativePages = relativePages;

            IncludeInSiteNavigation = true;
            DisableNavigationCascade = false;

            Log = new SubscriptionLogManageModel();
            Notify = new NotificationInitializeModel(NotificationEnums.NotificationModule.Page, new NotificationPageParameterModel { Id = -1 });

            SocialMessages = socialMediaTokenService.GetAvailableSocialMediaMessageModels();
            SEOScoring = new SEOScoringModel();
        }

        public PageManageModel(int? parentId, int? bodyTemplateId)
            : this()
        {
            int position;
            int relativePageId;
            var relativePages = _pageService.GetRelativePages(out position, out relativePageId, null, parentId);
            RelativePageId = relativePageId;
            RelativePages = relativePages;
            if (parentId.HasValue)
            {
                var parentPage = _pageService.GetById(parentId);
                PageTemplateId = parentPage.PageTemplateId;
                BodyTemplateId = bodyTemplateId.HasValue ? bodyTemplateId : parentPage.BodyTemplateId;
                Content = _bodyTemplateService.GetBodyTemplateContent(BodyTemplateId);
            }
        }

        public PageManageModel(Page page)
            : this()
        {
            int position;
            int relativePageId;
            var relativePages = _pageService.GetRelativePages(out position, out relativePageId, page.Id, page.ParentId);
            Position = position;
            Positions = EnumUtilities.GenerateSelectListItems<PageEnums.PagePosition>();
            RelativePageId = relativePageId;
            RelativePages = relativePages;

            Id = page.Id;

            if (page.Status == PageEnums.PageStatus.Draft)
            {
                Content = page.ContentWorking;
                Abstract = page.AbstractWorking;
            }
            else
            {
                Content = page.Content;
                Abstract = page.Abstract;
            }

            Title = page.Title;
            SeoTitle = page.SeoTitle;
            Keywords = page.Keywords;
            FriendlyUrl = page.FriendlyUrl;
            Description = page.Description;
            Status = page.Status;
            StatusList = _pageService.GetStatus();
            ParentId = page.ParentId;
            Parents = _pageService.GetPossibleParents(page.Id);
            FileTemplateId = page.FileTemplateId;
            FileTemplates = _fileTemplateService.GetFileTemplates(page.FileTemplateId);
            PageTemplateId = page.PageTemplateId;
            PageTemplates = _pageTemplateService.GetPageTemplateSelectList(page.PageTemplateId);
            BodyTemplateId = page.BodyTemplateId;
            BodyTemplates = _bodyTemplateService.GetBodyTemplates(BodyTemplateId);
            IncludeInSiteNavigation = page.IncludeInSiteNavigation;
            DisableNavigationCascade = page.DisableNavigationCascade;
            Tags = string.Join(",", page.PageTags.Select(t => t.Tag.Name));
            StartPublishingDate = page.StartPublishingDate;
            EndPublishingDate = page.EndPublishingDate;
            RecordOrder = page.RecordOrder;
            SSL = page.SSL;
            PageSecurityModels = _pageService.GetPageSecurities(page.Id);

            Log = new SubscriptionLogManageModel(SubscriptionEnums.SubscriptionModule.Page, new SubscriptionPageParameterModel { Id = page.Id });
            Notify = new NotificationInitializeModel(NotificationEnums.NotificationModule.Page, new NotificationPageParameterModel { Id = page.Id });

            var socialMessage = string.Format(SocialMessageFormat, page.Title, page.FriendlyUrl.ToAbsoluteUrl().ToTinyUrl());
            foreach (var item in SocialMessages)
            {
                item.PageTitle = page.Title;
                item.Message = socialMessage;
            }

            SEOScoring = _pageService.GetSEOScoringModel(page.Title, page.Description, page.Content, page.Keywords);
        }

        public PageManageModel(PageLog log)
            : this(log.Page)
        {
            Content = log.Content;
            Title = log.Title;
            SeoTitle = log.SeoTitle;
            FriendlyUrl = log.FriendlyUrl;
            Abstract = log.Abstract;
            Status = log.Status;
            FileTemplateId = log.FileTemplateId;
            PageTemplateId = log.PageTemplateId;
            BodyTemplateId = log.BodyTemplateId;
            IncludeInSiteNavigation = log.IncludeInSiteNavigation;
            DisableNavigationCascade = log.DisableNavigationCascade;
            StartPublishingDate = log.StartPublishingDate;
            EndPublishingDate = log.EndPublishingDate;

            foreach (var item in SocialMessages)
            {
                item.PageTitle = log.Title;
                item.Message = string.Format(SocialMessageFormat, log.Title, log.FriendlyUrl.ToAbsoluteUrl().ToTinyUrl());
            }

            SEOScoring = _pageService.GetSEOScoringModel(log.Title, log.Description, log.Content, log.Keywords);
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [StringLength(255)]
        [LocalizedDisplayName("Page_Field_Title")]
        public string Title { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Page_Field_SeoTitle")]
        public string SeoTitle { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Page_Field_Keywords")]
        public string Keywords { get; set; }

        [StringLength(255)]
        [LocalizedDisplayName("Page_Field_FriendlyUrl")]
        public string FriendlyUrl { get; set; }

        [LocalizedDisplayName("Page_Field_Abstract")]
        public string Abstract { get; set; }

        [LocalizedDisplayName("Page_Field_Description")]
        public string Description { get; set; }

        [LocalizedDisplayName("Page_Field_WordUpload")]
        public bool IsWordContent { get; set; }

        [RequiredIf("IsWordContent", true, "Page_Message_RequiredWordFile")]
        public HttpPostedFileBase File { get; set; }

        [LocalizedDisplayName("Page_Field_Content")]
        public string Content { get; set; }

        [LocalizedDisplayName("Page_Field_IncludeInSiteNavigation")]
        public bool IncludeInSiteNavigation { get; set; }

        [LocalizedDisplayName("Page_Field_DisableNavigationCascade")]
        public bool DisableNavigationCascade { get; set; }

        [LocalizedDisplayName("Page_Field_StartPublishingDate")]
        public DateTime? StartPublishingDate { get; set; }

        [DateGreaterThan("StartPublishingDate")]
        [LocalizedDisplayName("Page_Field_EndPublishingDate")]
        public DateTime? EndPublishingDate { get; set; }

        [LocalizedDisplayName("Page_Field_Tags")]
        public string Tags { get; set; }

        [LocalizedDisplayName("Page_Field_Parent")]
        public int? ParentId { get; set; }

        public IEnumerable<SelectListItem> Parents { get; set; }

        [LocalizedDisplayName("Page_Field_FileTemplateId")]
        public int? FileTemplateId { get; set; }

        public IEnumerable<SelectListItem> FileTemplates { get; set; }

        [LocalizedDisplayName("Page_Field_PageTemplateId")]
        public int? PageTemplateId { get; set; }

        public IEnumerable<SelectListItem> PageTemplates { get; set; }

        [LocalizedDisplayName("Page_Field_BodyTemplateId")]
        public int? BodyTemplateId { get; set; }

        public IEnumerable<SelectListItem> BodyTemplates { get; set; }

        [LocalizedDisplayName("Page_Field_Status")]
        public PageEnums.PageStatus Status { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }

        [LocalizedDisplayName("Page_Field_Position")]
        public int Position { get; set; }

        public IEnumerable<SelectListItem> Positions { get; set; }

        [LocalizedDisplayName("Page_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        [LocalizedDisplayName("Page_Field_SSL")]
        public bool SSL { get; set; }

        [LocalizedDisplayName("Page_Field_RelativePage")]
        public int? RelativePageId { get; set; }

        public IEnumerable<SelectListItem> RelativePages { get; set; }

        public List<PageSecurityModel> PageSecurityModels { get; set; }

        public List<string> IncludedStyles { get; set; }

        public SubscriptionLogManageModel Log { get; set; }

        public NotificationInitializeModel Notify { get; set; }

        public List<SocialMediaMessageModel> SocialMessages { get; set; }

        public SEOScoringModel SEOScoring { get; set; }

        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();

            /*
             * Check if friendly url is valid or not
             *  - Check if friendly url is match with other friendly url or not
             *  - Check if friendly url is match with other application routes or not 
             */
            FriendlyUrl = string.IsNullOrWhiteSpace(FriendlyUrl) ? Title.ToUrlString() : FriendlyUrl.ToUrlString();
            if (_pageService.IsFriendlyUrlExisted(Id, FriendlyUrl))
            {
                yield return
                    new ValidationResult(
                        localizedResourceService.T("Page_Message_ExistingFriendlyUrl"), new[] { "FriendlyUrl" });
            }
            else if (!FriendlyUrl.IsFriendlyUrlValid())
            {
                yield return new ValidationResult(localizedResourceService.T("Page_Message_FriendlyUrlMatchApplicationRoute"), new[] { "FriendlyUrl" });
            }

            if (_pageService.IsHomepageOffline(Id, Status))
            {
                yield return new ValidationResult(localizedResourceService.T("Page_Message_InvalidHomePageStatus"), new[] { "Status" });
            }

            /*Can only choose 1 type of template*/
            if (PageTemplateId.HasValue && FileTemplateId.HasValue)
            {
                yield return new ValidationResult(localizedResourceService.T("Page_Message_MultipleTemplates"), new[] { "PageTemplateId", "FileTemplateId" });
            }

            if (IsWordContent)
            {
                if (File == null)
                {
                    yield return
                        new ValidationResult(
                            localizedResourceService.T("Page_Message_WordFileMissing"), new[] { "File" });
                }
                else
                {
                    if (!FileUtilities.GetMimeMapping("docx").Equals(File.ContentType))
                    {
                        yield return
                            new ValidationResult(
                                localizedResourceService.T("Page_Message_WordFileWrongType"), new[] { "File" });
                    }
                }
            }
        }
    }
}
