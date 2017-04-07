using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.News.WidgetManageModels;
using EzCMS.Core.Models.News.Widgets;
using EzCMS.Core.Services.News;
using EzCMS.Entity.Core.Enums;
using System.Collections.Generic;
using System.Web;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.News
{
    public class NewsListingResolver : Widget
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
                Name = "News Listing",
                Widget = "NewsListing",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Get news listing and render using template",
                DefaultTemplate = "Default.NewsListing",
                Type = typeof(NewsListingWidget),
                ManageType = typeof(NewsListingWidgetManageModel),
                IsFavourite = true
            };
        }

        #endregion

        #region Private Properties

        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly INewsService _newsService;

        #endregion

        #region Constructors

        public NewsListingResolver()
        {
            _newsService = HostContainer.GetInstance<INewsService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "NewsCategoryId", Order = 1, Description = "The news category to load, this is optional parameter, if not defined then get all kinds of category")]
        public int? NewsCategoryId { get; set; }

        [WidgetParam(Name = "Total", Order = 2, Description = "The total news to get")]
        public int Total { get; set; }

        [WidgetParam(Name = "PageSize", Order = 3, Description = "The page size of listing, if not defined or = 0 then disable the paging")]
        public int PageSize { get; set; }

        private int PageIndex { get; set; }

        [WidgetParam(Name = "NewsType", Order = 4, Description = "Type of news: Hot News = 2, Not Hot News = 1, All = 0")]
        public int NewsType { get; set; }
        private NewsEnums.NewsType NewsTypeEnum
        {
            get
            {
                return NewsType.ToEnum<NewsEnums.NewsType>();
            }
        }

        [WidgetParam(Name = "NewsStatus", Order = 5, Description = "The status of news: Active = 1, Archive = 2, Inactive = 3")]
        public int NewsStatus { get; set; }

        private NewsEnums.NewsStatus NewsStatusEnum
        {
            get
            {
                return NewsStatus.ToEnum<NewsEnums.NewsStatus>();
            }
        }

        [WidgetParam(Name = "TemplateName", Order = 6, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * NewsCategoryId
             * * Total
             * * PageSize
             * * NewsType
             * * Template 
             */

            //NewsCategoryId
            if (parameters.Length > 1)
            {
                NewsCategoryId = parameters[1].ToInt(0);
            }

            //Total
            if (parameters.Length > 2)
            {
                Total = parameters[2].ToInt(0);
            }

            //PageSize
            if (parameters.Length > 3)
            {
                PageSize = parameters[3].ToInt(0);
            }

            //NewsType
            if (parameters.Length > 4)
            {
                NewsType = parameters[4].ToInt((int)NewsEnums.NewsType.All);
            }

            //News Status
            if (parameters.Length > 5)
            {
                NewsStatus = parameters[5].ToInt((int)NewsEnums.NewsStatus.Active);
            }

            //Template 
            if (parameters.Length > 6)
            {
                Template = parameters[6];
            }

            var context = HttpContext.Current.Request;
            PageIndex = context.QueryString["page"].ToInt(0);
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
                parameters.AddRange(new List<object> { NewsCategoryId, Total, PageSize, NewsType, NewsStatus, Template });
            }
            else if (NewsStatus != (int)NewsEnums.NewsStatus.Active)
            {
                parameters.AddRange(new List<object> { NewsCategoryId, Total, PageSize, NewsType, NewsStatus });
            }
            else if (NewsType != (int)NewsEnums.NewsType.All)
            {
                parameters.AddRange(new List<object> { NewsCategoryId, Total, PageSize, NewsType });
            }
            else if (PageSize > 0)
            {
                parameters.AddRange(new List<object> { NewsCategoryId, Total, PageSize });
            }
            else if (Total > 0)
            {
                parameters.AddRange(new List<object> { NewsCategoryId, Total });
            }
            else if (NewsCategoryId > 0)
            {
                parameters.AddRange(new List<object> { NewsCategoryId });
            }

            return string.Join(EzCMSContants.WidgetSeparator, parameters).ToWidgetFormat();
        }

        #endregion

        /// <summary>
        /// Render News widget
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override string Render(string[] parameters)
        {
            ParseParams(parameters);

            var model = _newsService.GetNewsListing(PageIndex, PageSize, Total, NewsTypeEnum, NewsCategoryId, NewsStatusEnum);

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
