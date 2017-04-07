using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.ClientNavigations.WidgetManageModels;
using EzCMS.Core.Models.ClientNavigations.Widgets;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Services.ClientNavigations;
using EzCMS.Core.Services.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.Navigations
{
    public class NavigationResolver : Widget
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
                Name = "Navigations",
                Widget = "Navigations",
                WidgetType = WidgetType.Navigation,
                Description = "Navigation widget for rendering responsive Navigations",
                DefaultTemplate = "Default.Navigations",
                Type = typeof(List<ITree<NavigationNodeModel>>),
                ManageType = typeof(NavigationsWidgetManageModel),
                IsFavourite = true
            };
        }

        #endregion

        #region Private Properties
        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly IClientNavigationService _clientNavigationService;
        #endregion

        #region Constructors

        public NavigationResolver()
        {
            _clientNavigationService = HostContainer.GetInstance<IClientNavigationService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            ShowParentNavigation = false;
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "ParentId", Order = 1, Description = "The parent id to load as root, if not defined or empty then root will be all")]
        public int? ParentId { get; set; }

        [WidgetParam(Name = "ShowParentNavigation", Order = 2, Description = "Parameter to check if we show parent as first node of Navigation or not")]
        public bool ShowParentNavigation { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 3, Description = "The template name of Navigations, this is optional parameter, if not defined then get the default Template ")]
        public string Template { get; set; }

        /// <summary>
        /// Parse params from parameters
        /// </summary>
        /// <param name="parameters"></param>
        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Parent Id
             * * Show Parent
             * * Parent Template 
             */

            //ParentId
            if (parameters.Length > 1 && !string.IsNullOrEmpty(parameters[1]))
            {
                ParentId = parameters[1].ToNullableInt();
            }

            //ShowParentNavigation
            if (parameters.Length > 2 && !string.IsNullOrEmpty(parameters[2]) && ParentId.HasValue)
            {
                ShowParentNavigation = parameters[2].ToBool(false);
            }

            //TemplateName
            if (parameters.Length > 3 && !string.IsNullOrEmpty(parameters[3]))
            {
                Template = parameters[3];
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
                parameters.AddRange(new List<object> { ParentId, ShowParentNavigation, Template });
            }
            else if (ShowParentNavigation)
            {
                parameters.AddRange(new List<object> { ParentId, ShowParentNavigation });
            }
            else if (ParentId > 0)
            {
                parameters.AddRange(new List<object> { ParentId });
            }

            return string.Join(EzCMSContants.WidgetSeparator, parameters).ToWidgetFormat();
        }

        #endregion

        /// <summary>
        /// Render dynamic Navigation
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override string Render(string[] parameters)
        {
            //Initialize parameter
            ParseParams(parameters);

            #region Try get template cache

            /*
             * Check storing Navigation result in cache
             * If all the Navigations are not updated then get the cache result
             * If not rebuild the data and recache
             */
            var cacheName = string.Join("_", parameters);
            var cacheNavigation = StateManager.GetApplication<CacheNavigation>(cacheName);

            if (cacheNavigation != null)
            {
                var template = _widgetTemplateService.GetTemplate(Template) ??
                               _widgetTemplateService.GetTemplate(GetSetup().DefaultTemplate);
                if (template == null)
                {
                    throw new Exception(string.Format("Template {0} is not found", Template));
                }

                var lastCreated = _clientNavigationService.GetAll().Max(t => t.Created);
                var lastUpdated = _clientNavigationService.GetAll().Max(t => t.LastUpdate) ?? DateTime.MinValue;
                var dataCacheTime = lastCreated > lastUpdated ? lastCreated : lastUpdated;

                lastCreated = template.Created;
                lastUpdated = template.LastUpdate ?? DateTime.MinValue;
                var templateCacheTime = lastCreated > lastUpdated ? lastCreated : lastUpdated;

                if (cacheNavigation.DataCacheTime == dataCacheTime && cacheNavigation.TemplateCacheTime == templateCacheTime)
                {
                    return cacheNavigation.Content;
                }
            }

            #endregion

            var renderTemplate = _widgetTemplateService.GetTemplateByName(Template) ??
                                 _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            //Get Navigations
            var data = _clientNavigationService.GenerateNavigations(ParentId, ShowParentNavigation);

            /*
             * Create cache version of this widget and store in Application
             */
            cacheNavigation = new CacheNavigation
            {
                Content = _widgetTemplateService.ParseTemplate(renderTemplate, data),
                DataCacheTime = DateTime.UtcNow,
                TemplateCacheTime = DateTime.UtcNow
            };
            HttpContext.Current.Application[cacheName] = cacheNavigation;

            return cacheNavigation.Content;
        }
    }
}