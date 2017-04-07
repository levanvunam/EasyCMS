using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.NewsCategories.WidgetManageModels;
using EzCMS.Core.Models.NewsCategories.Widgets;
using EzCMS.Core.Services.NewsCategories;
using System.Collections.Generic;
using System.Web;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.NewsCategories
{
    public class NewsCategoryDetailsResolver : Widget
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
                Name = "News Category Details",
                Widget = "NewsCategoryDetails",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Get news category information and render using template",
                DefaultTemplate = "Default.NewsCategoryDetails",
                Type = typeof(NewsCategoryWidget),
                ManageType = typeof(NewsCategoryWidgetManageModel)
            };
        }

        #endregion

        #region Private Properties
        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly INewsCategoryService _newsCategoryService;
        #endregion

        #region Constructors

        public NewsCategoryDetailsResolver()
        {
            _newsCategoryService = HostContainer.GetInstance<INewsCategoryService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "CategoryId", Order = 1, Description = "The id of category that need to generate details")]
        public int CategoryId { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 2, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * CategoryId
             * * Template 
             */

            //Template 
            if (parameters.Length > 1)
            {
                CategoryId = parameters[1].ToInt(0);
            }

            if (CategoryId == 0)
            {
                CategoryId = HttpContext.Current.Request.QueryString["CategoryId"].ToInt(0);
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
                parameters.AddRange(new List<object> { CategoryId, Template });
            }
            else if (CategoryId > 0)
            {
                parameters.AddRange(new List<object> { CategoryId });
            }

            return string.Join(EzCMSContants.WidgetSeparator, parameters).ToWidgetFormat();
        }

        #endregion

        /// <summary>
        /// Render Category widget
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override string Render(string[] parameters)
        {
            ParseParams(parameters);

            var model = _newsCategoryService.GetCategoryModel(CategoryId);

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
