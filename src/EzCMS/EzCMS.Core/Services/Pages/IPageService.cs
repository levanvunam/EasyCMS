using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using Ez.Framework.Utilities.Web.Models;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Models.Pages;
using EzCMS.Core.Models.Pages.Widgets;
using EzCMS.Core.Models.Pages.Widgets.Member;
using EzCMS.Core.Models.Pages.Widgets.PageSearch;
using EzCMS.Core.Models.UserGroups;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Pages
{
    [Register(Lifetime.PerInstance)]
    public interface IPageService : IBaseService<Page>
    {
        #region Logs

        /// <summary>
        /// Get logs
        /// </summary>
        /// <param name="id"></param>
        /// <param name="total"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        PageLogListingModel GetLogs(int id, int total = 0, int index = 1);

        #endregion

        /// <summary>
        /// Check if current user has permission to create new page or edit page
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        bool CanCurrentUserCreateOrEditPage(int? pageId = null);

        /// <summary>
        /// Get web page information
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        WebPageInformationModel GetWebPageInformationModel(int pageId);

        #region Get

        /// <summary>
        /// Get page by friendly url
        /// </summary>
        /// <param name="friendlyUrl"></param>
        /// <returns></returns>
        Page GetPage(string friendlyUrl);

        /// <summary>
        /// Get all accessable pages
        /// </summary>
        /// <returns></returns>
        IQueryable<Page> GetAccessablePages();

        /// <summary>
        /// Get accessable page select list
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetAccessablePageSelectList();

        /// <summary>
        /// Get all editalbe groups of page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<GroupItem> GetEditableGroups(int? id);

        /// <summary>
        /// Get all viewable groups of page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<GroupItem> GetViewableGroups(int? id);

        #endregion

        #region Grid Search

        /// <summary>
        /// Search pages
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchPages(JqSearchIn si, PageSearchModel model);

        /// <summary>
        /// Export pages
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, PageSearchModel model);

        /// <summary>
        /// Search child pages of page
        /// </summary>
        /// <param name="si"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchChildrenPages(JqSearchIn si, int parentId);

        /// <summary>
        /// Export pages
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        HSSFWorkbook ExportsChildrenPages(JqSearchIn si, GridExportMode gridExportMode, int parentId);

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="pages"></param>
        /// <returns></returns>
        IQueryable<PageModel> Maps(IQueryable<Page> pages);

        #endregion

        #region Manage

        /// <summary>
        /// Create a page manage model and populate data from parent and body template if provided
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="bodyTemplateId"></param>
        /// <returns></returns>
        PageManageModel GetPageManageModelWithParent(int? parentId = null, int? bodyTemplateId = null);

        /// <summary>
        /// Get page manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PageManageModel GetPageManageModel(int? id = null);

        /// <summary>
        /// Get page manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PageManageModel GetPageManageModelByLogId(int? id = null);

        /// <summary>
        /// Save page
        /// </summary>
        /// <param name="model"></param>
        /// <param name="confirmedChangeUrl"></param>
        /// <returns></returns>
        ResponseModel SavePageManageModel(PageManageModel model, bool confirmedChangeUrl = false);

        #region Methods

        /// <summary>
        /// Save page order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SavePageOrder(PageOrderSetupModel model);

        /// <summary>
        /// Get page securities
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        List<PageSecurityModel> GetPageSecurities(int? pageId = null);

        /// <summary>
        /// Change page to be home page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel ChangeHomePage(int id);

        #endregion

        #region SEO Scoring

        /// <summary>
        /// Get SEO scoring
        /// </summary>
        /// <param name="content"></param>
        /// <param name="keywords"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        SEOScoringModel GetSEOScoringModel(string title, string description, string content, string keywords);

        /// <summary>
        /// Get SEO score when changing title
        /// </summary>
        /// <param name="title"></param>
        /// <param name="seoScoringSetting"></param>
        /// <returns></returns>
        PageEnums.SEOScore GetTitleChangedSEOScore(string title, SEOScoringSetting seoScoringSetting = null);

        /// <summary>
        /// Get SEO score when changing description
        /// </summary>
        /// <param name="description"></param>
        /// <param name="seoScoringSetting"></param>
        /// <returns></returns>
        PageEnums.SEOScore GetDescriptionChangedSEOScore(string description, SEOScoringSetting seoScoringSetting = null);

        /// <summary>
        /// Get SEO score when changing content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="keywords"></param>
        /// <param name="keywordWeightScore"></param>
        /// <param name="keywordBoldedScore"></param>
        /// <param name="headingScore"></param>
        /// <param name="altScore"></param>
        /// <param name="seoScoringSetting"></param>
        void GetContentChangedSEOScore(string content, string keywords, out PageEnums.SEOScore keywordWeightScore,
            out PageEnums.SEOScore keywordBoldedScore, out PageEnums.SEOScore headingScore,
            out PageEnums.SEOScore altScore, SEOScoringSetting seoScoringSetting = null);

        /// <summary>
        /// Get SEO score when changing keywords
        /// </summary>
        /// <param name="content"></param>
        /// <param name="keywords"></param>
        /// <param name="keywordCountScore"></param>
        /// <param name="keywordWeightScore"></param>
        /// <param name="keywordBoldedScore"></param>
        /// <param name="headingScore"></param>
        /// <param name="altScore"></param>
        /// <param name="seoScoringSetting"></param>
        /// <returns></returns>
        void GetKeywordsChangedSEOScore(string content, string keywords, out PageEnums.SEOScore keywordCountScore,
            out PageEnums.SEOScore keywordWeightScore, out PageEnums.SEOScore keywordBoldedScore,
            out PageEnums.SEOScore headingScore, out PageEnums.SEOScore altScore,
            SEOScoringSetting seoScoringSetting = null);

        #endregion

        #endregion

        #region Delete Page

        /// <summary>
        /// Get all pages that reference page via url
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        IQueryable<Page> GetReferencePages(Page page);

        /// <summary>
        /// Get page delete model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PageDeleteModel GetPageDeleteModel(int id);

        /// <summary>
        /// Delete page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel DeletePage(PageDeleteModel model);

        /// <summary>
        /// Remove relationship between page and page template
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="pageTemplateId"></param>
        /// <returns></returns>
        ResponseModel DeletePagePageTemplateMapping(int pageId, int pageTemplateId);

        /// <summary>
        /// Remove relationship between page and file template
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="fileTemplateId"></param>
        /// <returns></returns>
        ResponseModel DeletePageFileTemplateMapping(int pageId, int fileTemplateId);

        /// <summary>
        /// Remove relationship between page and body template
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="bodyTemplateId"></param>
        /// <returns></returns>
        ResponseModel DeletePageBodyTemplateMapping(int pageId, int bodyTemplateId);

        #endregion

        #region Select List

        /// <summary>
        /// Get possible parents
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetPossibleParents(int? id = null);

        /// <summary>
        /// Get status
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetStatus();

        /// <summary>
        /// Get relative pages
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetRelativePages(int? pageId = null, int? parentId = null);

        /// <summary>
        /// Get relative pages
        /// </summary>
        /// <param name="position"></param>
        /// <param name="relativePageId"></param>
        /// <param name="pageId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetRelativePages(out int position, out int relativePageId, int? pageId = null,
            int? parentId = null);

        #endregion

        #region Widgets

        #region Members & Breadcrumbs

        /// <summary>
        /// Get member widgets
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        MembersWidget GetMembers(int? pageId);

        /// <summary>
        /// Get breadcrumbs widget
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        BreadcrumbsWidget GetBreadcrumbs(int? pageId);

        #endregion

        #region Search Page

        /// <summary>
        /// Get search page widget
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        PageSearchWidget SearchPages(string keyword);

        #endregion

        #region Page Link

        /// <summary>
        /// Get page link widget
        /// </summary>
        /// <param name="id"></param>
        /// <param name="className"></param>
        /// <param name="title"></param>
        /// <param name="renderHtmlLink"></param>
        /// <returns></returns>
        PageLinkWidget GetPageLinkWidget(int id, bool renderHtmlLink, string className, string title);

        #endregion

        #region Page Content

        /// <summary>
        /// Get page render model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PageRenderModel GetPageRenderModel(int id);

        #endregion

        #endregion

        #region Validation

        /// <summary>
        /// Check if home page is offline
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        bool IsHomepageOffline(int? pageId, PageEnums.PageStatus status);

        /// <summary>
        /// Check if friendly url exists
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="friendlyUrl"></param>
        /// <returns></returns>
        bool IsFriendlyUrlExisted(int? pageId, string friendlyUrl);

        #endregion

        #region Render Content

        /// <summary>
        /// Render content
        /// </summary>
        /// <param name="id"></param>
        /// <param name="showDraft"></param>
        /// <returns></returns>
        PageRenderModel RenderContent(int? id, bool showDraft = false);

        /// <summary>
        /// Render content
        /// </summary>
        /// <param name="url"></param>
        /// <param name="showDraft"></param>
        /// <returns></returns>
        PageRenderModel RenderContent(string url, bool showDraft = false);

        /// <summary>
        /// Setup page read
        /// </summary>
        /// <param name="pageId"></param>
        ResponseModel SetupPageTracking(int pageId);

        #endregion

        #region Update Reference Url

        /// <summary>
        /// Get page confirm change url model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newFriendlyUrl"></param>
        /// <returns></returns>
        PageConfirmChangeUrlModel GetPageConfirmChangeUrlModel(int id, string newFriendlyUrl);

        /// <summary>
        /// Get all pages that reference page via friendly url in content
        /// </summary>
        /// <param name="friendlyUrl"></param>
        /// <returns></returns>
        List<Page> GetReferencePagesViaLink(string friendlyUrl);

        #endregion

        #region Details

        /// <summary>
        /// Get page details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PageDetailModel GetPageDetailModel(int id);

        /// <summary>
        /// Update page data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdatePageData(XEditableModel model);

        #endregion
    }
}