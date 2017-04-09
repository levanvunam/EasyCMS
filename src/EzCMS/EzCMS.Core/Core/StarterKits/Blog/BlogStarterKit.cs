using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Framework.EmbeddedResource;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Framework.StarterKits;
using EzCMS.Core.Framework.StarterKits.Attributes;
using EzCMS.Core.Framework.StarterKits.Interfaces;
using EzCMS.Core.Models.ClientNavigations.Widgets;
using EzCMS.Core.Services.StarterKits;
using EzCMS.Core.Services.Tree;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using Style = EzCMS.Entity.Entities.Models.Style;

namespace EzCMS.Core.Core.StarterKits.Blog
{
    [StarterKit(Name = "Blog", Description = "Blog Starter Kit", ImageName = "BlogTheme.jpg")]
    public class BlogStarterKit : IStarterKit
    {
        public string GetTemplateFolder()
        {
            return "Blog";
        }

        public void Setup()
        {
            var starterKitService = HostContainer.GetInstance<IStarterKitService>();

            #region Page Templates

            // Insert master template
            var master = new PageTemplate
            {
                Name = "Master",
                Content = StarterKitHelper.GetEzCMSResource("Master.cshtml", GetTemplateFolder(), ResourceTypeEnums.PageTemplates),
                IsDefaultTemplate = true
            };
            starterKitService.InsertPageTemplate(master);

            #endregion

            #region Page Initialize

            // Insert home page
            var homePage = new Page
            {
                Title = "Home",
                Abstract = "Home page",
                Content = StarterKitHelper.GetEzCMSResource("HomePage.cshtml", GetTemplateFolder(), ResourceTypeEnums.Pages),
                FriendlyUrl = "home",
                PageTemplateId = master.Id,
                IsHomePage = true,
                Status = PageEnums.PageStatus.Online,
                IncludeInSiteNavigation = true,
                DisableNavigationCascade = false,
                RecordOrder = 1
            };
            starterKitService.InsertPage(homePage);

            var aboutUs = new Page
            {
                Title = "About Us",
                Abstract = "About Us",
                Content = StarterKitHelper.GetEzCMSResource("AboutUs.cshtml", GetTemplateFolder(), ResourceTypeEnums.Pages),
                FriendlyUrl = "about-us",
                PageTemplateId = master.Id,
                IsHomePage = false,
                Status = PageEnums.PageStatus.Online,
                IncludeInSiteNavigation = true,
                DisableNavigationCascade = false,
                RecordOrder = 2
            };
            starterKitService.InsertPage(aboutUs);

            var contactUs = new Page
            {
                Title = "Contact Us",
                Abstract = "Contact Us",
                Content = StarterKitHelper.GetEzCMSResource("ContactUs.cshtml", GetTemplateFolder(), ResourceTypeEnums.Pages),
                FriendlyUrl = "contact-us",
                PageTemplateId = master.Id,
                IsHomePage = false,
                Status = PageEnums.PageStatus.Online,
                IncludeInSiteNavigation = true,
                DisableNavigationCascade = false,
                RecordOrder = 3
            };
            starterKitService.InsertPage(contactUs);

            #region Insert other pages

            //Page not found
            var pageNotFound = new Page
            {
                Title = "Page Not Found",
                Abstract = "Page Not Found",
                Content = StarterKitHelper.GetEzCMSResource("PageNotFound.cshtml", GetTemplateFolder(), ResourceTypeEnums.Pages),
                FriendlyUrl = "Page-Not-Found",
                PageTemplateId = master.Id,
                IsHomePage = false,
                Status = PageEnums.PageStatus.Online,
                IncludeInSiteNavigation = false,
                DisableNavigationCascade = true,
                RecordOrder = 4
            };
            starterKitService.InsertPage(pageNotFound);

            //Page error
            var pageError = new Page
            {
                Title = "Page Error",
                Abstract = "Page Error",
                Content = StarterKitHelper.GetEzCMSResource("PageError.cshtml", GetTemplateFolder(), ResourceTypeEnums.Pages),
                FriendlyUrl = "Page-Error",
                PageTemplateId = master.Id,
                IsHomePage = false,
                Status = PageEnums.PageStatus.Online,
                IncludeInSiteNavigation = false,
                DisableNavigationCascade = true,
                RecordOrder = 3
            };
            starterKitService.InsertPage(pageError);

            //Site map
            var siteMap = new Page
            {
                Title = "Site Map",
                Abstract = "Site Map",
                Content = StarterKitHelper.GetEzCMSResource("SiteMap.cshtml", GetTemplateFolder(), ResourceTypeEnums.Pages),
                FriendlyUrl = "Site-Map",
                PageTemplateId = master.Id,
                IsHomePage = false,
                Status = PageEnums.PageStatus.Online,
                IncludeInSiteNavigation = false,
                DisableNavigationCascade = true,
                RecordOrder = 5
            };
            starterKitService.InsertPage(siteMap);

            //Search page
            var pageSearch = new Page
            {
                Title = "Search",
                Abstract = "Search",
                Content = StarterKitHelper.GetEzCMSResource("Search.cshtml", GetTemplateFolder(), ResourceTypeEnums.Pages),
                FriendlyUrl = "Search",
                PageTemplateId = master.Id,
                IsHomePage = false,
                Status = PageEnums.PageStatus.Online,
                IncludeInSiteNavigation = false,
                DisableNavigationCascade = true,
                RecordOrder = 6
            };
            starterKitService.InsertPage(pageSearch);

            #endregion

            #endregion

            #region Styles

            //Add style
            var templateStyle = new Style
            {
                Name = "template",
                Content = StarterKitHelper.GetEzCMSResource("template.css", GetTemplateFolder(), ResourceTypeEnums.Styles),
                IncludeIntoEditor = true
            };
            starterKitService.InsertStyle(templateStyle);

            //Add protected document css
            var protectedDocumentStyle = new Style
            {
                Name = "protected-document",
                Content = StarterKitHelper.GetEzCMSResource("protected-document.css", GetTemplateFolder(), ResourceTypeEnums.Styles)
            };
            starterKitService.InsertStyle(protectedDocumentStyle);

            //Add bootstrap css
            var bootstrapStyle = new Style
            {
                Name = "bootstrap",
                Content = string.Empty,
                CdnUrl = "/Content/Plugins/Bootstrap/bootstrap.min.css",
                IncludeIntoEditor = true
            };
            starterKitService.InsertStyle(bootstrapStyle);

            #endregion

            #region Scripts

            var contactMe = new Script
            {
                Name = "contact-me",
                Content = StarterKitHelper.GetEzCMSResource("contact_me.js", GetTemplateFolder(), ResourceTypeEnums.Scripts)
            };
            starterKitService.InsertScript(contactMe);

            var main = new Script
            {
                Name = "main",
                Content = StarterKitHelper.GetEzCMSResource("main.js", GetTemplateFolder(), ResourceTypeEnums.Scripts)
            };
            starterKitService.InsertScript(main);


            var validation = new Script
            {
                Name = "jqBootstrapValidation",
                Content = StarterKitHelper.GetEzCMSResource("jqBootstrapValidation.js", GetTemplateFolder(), ResourceTypeEnums.Scripts)
            };
            starterKitService.InsertScript(validation);

            #endregion

            #region Widget Templates

            string dataType = typeof(List<ITree<NavigationNodeModel>>).AssemblyQualifiedName;
            string navigationContent = StarterKitHelper.GetEzCMSResource("Navigations.Navigations.cshtml", GetTemplateFolder(), ResourceTypeEnums.WidgetTemplates);
            string navigationScript = StarterKitHelper.GetEzCMSResource("Navigations.Navigations.Script.cshtml",
                GetTemplateFolder(), ResourceTypeEnums.WidgetTemplates);
            var navigations = new WidgetTemplate
            {
                Name = "Navigations",
                DataType = dataType,
                Widget = "Navigations",
                Content = navigationContent,
                Script = navigationScript,
                FullContent = WidgetHelper.GetFullTemplate(navigationContent, string.Empty, navigationScript, dataType, string.Empty)
            };
            starterKitService.InsertWidgetTemplate(navigations);

            #endregion
        }
    }
}
