using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Subscriptions.WidgetManageModels;
using EzCMS.Core.Models.Subscriptions.Widgets;
using EzCMS.Core.Models.Subscriptions.ModuleParameters;
using EzCMS.Core.Services.Pages;
using EzCMS.Core.Services.Subscriptions;
using EzCMS.Entity.Core.Enums;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.Subscriptions
{
    public class SubscriptionResolver : Widget
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
                Name = "Subscription",
                Widget = "Subscription",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Generate subscription link in site",
                DefaultTemplate = "Default.Subscription",
                Type = typeof(SubscriptionWidget),
                ManageType = typeof(SubscriptionWidgetManageModel)
            };
        }

        #endregion

        #region Private Properties
        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IPageService _pageService;
        private readonly ISiteSettingService _siteSettingService;
        #endregion

        #region Constructors

        public SubscriptionResolver()
        {
            _subscriptionService = HostContainer.GetInstance<ISubscriptionService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            _pageService = HostContainer.GetInstance<IPageService>();
            _siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "Module", Order = 1, Description = "The subscription module")]
        public int Module { get; set; }

        private SubscriptionEnums.SubscriptionModule ModuleEnum
        {
            get
            {
                return Module.ToEnum<SubscriptionEnums.SubscriptionModule>();
            }
        }

        private dynamic Parameters { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 2, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Module
             * * Template 
             */
            Parameters = null;

            //Module
            if (parameters.Length > 1)
            {
                Module = parameters[1].ToInt((int)SubscriptionEnums.SubscriptionModule.Page);
            }

            switch (ModuleEnum)
            {
                case SubscriptionEnums.SubscriptionModule.Page:
                    var pageId = WorkContext.ActivePageId;
                    var page = _pageService.GetById(pageId);
                    if (page != null)
                    {
                        Parameters = new SubscriptionPageParameterModel(page);
                    }
                    break;
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
                GetSetup().Widget, Module
            };

            if (!string.IsNullOrEmpty(Template))
            {
                parameters.AddRange(new List<object> { Module, Template });
            }

            return string.Join(EzCMSContants.WidgetSeparator, parameters).ToWidgetFormat();
        }

        #endregion

        /// <summary>
        /// Render testimonial widget
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override string Render(string[] parameters)
        {
            var subscriptionConfiguration = _siteSettingService.LoadSetting<SubscriptionConfigurationSetting>();
            if (!subscriptionConfiguration.DisableNotifySubscribers)
            {
                ParseParams(parameters);

                var model = _subscriptionService.GenerateSubscription(ModuleEnum, WorkContext.CurrentContact.Email, Parameters);

                var template = _widgetTemplateService.GetTemplateByName(Template) ??
                               _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

                return _widgetTemplateService.ParseTemplate(template, model);
            }

            return string.Empty;
        }
    }
}
