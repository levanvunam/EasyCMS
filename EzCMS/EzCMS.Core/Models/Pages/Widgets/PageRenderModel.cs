using Ez.Framework.Configurations;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Reflection.Attributes;
using Ez.Framework.IoC;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.FileTemplates;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Models.Pages.Widgets
{
    public class PageRenderModel
    {
        #region Constructors

        public PageRenderModel()
        {
            HierarchyIds = new List<int>();
            GroupIds = new List<int>();

            var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
            var company = siteSettingService.LoadSetting<CompanySetupSetting>();

            //Set current keywords by site keywords
            Keywords = company.Keywords;
            SiteTitle = company.SiteTitle;
        }

        public PageRenderModel(Page page, bool showDraft = false)
            : this()
        {
            Id = page.Id;
            Title = string.IsNullOrEmpty(page.SeoTitle) ? page.Title : page.SeoTitle;
            Description = page.Description;

            //Only use the site keywords when page keywords is empty
            if (string.IsNullOrWhiteSpace(page.Keywords))
            {
                Keywords = page.Keywords;
            }

            //Remove widget tag from content before rendering
            Content = page.Content.RemoveWidgetTag();

            //Check if current route is for file template
            if (page.FileTemplateId.HasValue)
            {
                ResponseCode = PageEnums.PageResponseCode.FileTemplateRedirect;
                FileTemplateModel = new TemplateModel
                {
                    Name = page.FileTemplate.Name,
                    Action = page.FileTemplate.Action,
                    Area = page.FileTemplate.Area,
                    Controller = page.FileTemplate.Controller,
                    Parameters = string.Format("{0}={1}&{2}={3}&{4}", EzCMSContants.ActivePageId, page.Id, EzCMSContants.FileTemplateId, page.FileTemplateId, page.FileTemplate.Parameters),
                };
            }

            if (!string.IsNullOrEmpty(page.Hierarchy))
            {
                HierarchyIds = page.Hierarchy.Split(new[] { FrameworkConstants.IdSeparator }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.ToInt()).ToList();
            }

            Status = page.Status;
            ShowDraft = showDraft;
            ResponseCode = PageEnums.PageResponseCode.Ok;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        [IgnoreInDropdown]
        public TemplateModel FileTemplateModel { get; set; }

        public string Title { get; set; }

        [UsingRaw]
        public string Content { get; set; }

        public string Keywords { get; set; }

        public string Description { get; set; }

        public PageEnums.PageStatus Status { get; set; }

        [IgnoreInDropdown]
        public bool ShowDraft { get; set; }

        [IgnoreInDropdown]
        public List<int> HierarchyIds { get; set; }

        #region Security

        [IgnoreInDropdown]
        public bool IsLoggedIn { get; set; }

        [IgnoreInDropdown]
        public bool CanView { get; set; }

        [IgnoreInDropdown]
        public bool CanEdit { get; set; }

        [IgnoreInDropdown]
        public List<int> GroupIds { get; set; }

        #endregion

        [IgnoreInDropdown]
        public PageEnums.PageResponseCode ResponseCode { get; set; }

        [IgnoreInDropdown]
        public string Redirect301Url { get; set; }

        #region Company Setup

        public string SiteTitle { get; set; }

        #endregion

        #endregion
    }
}
