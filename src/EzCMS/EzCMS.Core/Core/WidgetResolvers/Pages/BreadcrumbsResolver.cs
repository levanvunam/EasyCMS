using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Pages.WidgetManageModels;
using EzCMS.Core.Models.Pages.Widgets.Member;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.Pages;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.Pages
{
    public class BreadcrumbsResolver : Widget
    {
        #region Setup

        /// <summary>
        /// Get widget information
        /// </summary>
        /// <returns></returns>
        public override WidgetSetupModel GetSetup()
        {
            return new WidgetSetupModel
            {
                Name = "Page Breadcrumbs",
                Widget = "Breadcrumbs",
                WidgetType = WidgetType.Navigation,
                Description = "Load breadcrumbs for page",
                DefaultTemplate = "Default.Breadcrumbs",
                Type = typeof(BreadcrumbsWidget),
                ManageType = typeof(MemberWidgetManageModel),
                IsFavourite = true
            };
        }

        #endregion

        #region Private Properties
        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly IPageService _pageService;
        private readonly IEzCMSLocalizedResourceService _localizedResourceService;

        #endregion

        #region Constructors

        public BreadcrumbsResolver()
        {
            _pageService = HostContainer.GetInstance<IPageService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            _localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();

        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "PageId", Order = 1, Description = "The id of page that need to generate breadcrumbs")]
        public int? PageId { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 2, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Page Id
             * * Template 
             */

            //Page Id
            if (parameters.Length > 1)
            {
                PageId = parameters[1].ToNullableInt();
            }

            if (!PageId.HasValue || PageId == 0)
            {
                PageId = WorkContext.ActivePageId;
            }

            //Template 
            if (parameters.Length > 2)
            {
                Template = parameters[2];
            }
        }

        #endregion

        #region Generate Widget

        /// <summary>
        /// Generate full widget from params
        /// </summary>
        /// <returns></returns>
        public override string GenerateFullWidget()
        {
            var parameters = new List<object>
            {
                GetSetup().Widget
            };

            if (!string.IsNullOrEmpty(Template))
            {
                parameters.AddRange(new List<object> { PageId, Template });
            }
            else if (PageId > 0)
            {
                parameters.AddRange(new List<object> { PageId });
            }

            return string.Join(EzCMSContants.WidgetSeparator, parameters).ToWidgetFormat();
        }

        #endregion

        /// <summary>
        /// Render widget
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override string Render(string[] parameters)
        {
            ParseParams(parameters);

            var model = _pageService.GetBreadcrumbs(PageId);

            if (model == null)
            {
                return _localizedResourceService.T("Widget_Breadcrumb_Message_InvalidPageId");
            }

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
