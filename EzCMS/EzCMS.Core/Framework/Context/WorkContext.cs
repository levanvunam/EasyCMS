using Ez.Framework.Configurations;
using Ez.Framework.Core.Context;
using Ez.Framework.Core.IoC;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Time;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context.Helper;
using EzCMS.Core.Framework.Mvc.Models;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Models.LocalizedResources;
using EzCMS.Core.Models.SiteSettings;
using EzCMS.Core.Models.SiteSetup;
using EzCMS.Core.Models.SlideInHelps;
using EzCMS.Core.Models.Users;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Services.SiteSettings;
using System;
using System.Collections.Generic;
using System.Resources;

namespace EzCMS.Core.Framework.Context
{
    public class WorkContext : EzWorkContext
    {
        #region System

        /// <summary>
        /// Get current user stored in cuture
        /// </summary>
        public static int? ActivePageId
        {
            get
            {
                var activePageId = StateManager.GetContextItem<int?>(EzCMSContants.ActivePageId);
                if (activePageId.HasValue)
                {
                    StateManager.SetContextItem(EzCMSContants.ActivePageId, activePageId.Value);
                }

                return activePageId;
            }
            set
            {
                StateManager.SetContextItem(EzCMSContants.ActivePageId, value);
            }
        }

        /// <summary>
        /// List of 404 url that the error handled already
        /// </summary>
        public static List<string> NotFoundHandledUrls
        {
            get
            {
                return StateManager.GetApplication(EzCMSContants.NotFoundHandledUrls, new List<string>());
            }
            set
            {
                StateManager.SetApplication(EzCMSContants.NotFoundHandledUrls, value);
            }
        }

        /// <summary>
        /// Get all current widgets of application
        /// </summary>
        public static List<WidgetSetupModel> Widgets
        {
            get
            {
                return StateManager.GetApplication(EzCMSContants.ApplicationWidgets, new List<WidgetSetupModel>());
            }
            set
            {
                StateManager.SetApplication(EzCMSContants.ApplicationWidgets, value);
            }
        }

        /// <summary>
        /// Localize resource dictionary
        /// </summary>
        public static List<LocalizeDictionaryItem> LocalizedResourceDictionary
        {
            get
            {
                return StateManager.GetApplication(EzCMSContants.LocalizedResourceDictionary, new List<LocalizeDictionaryItem>());
            }
            set
            {
                StateManager.SetApplication(EzCMSContants.LocalizedResourceDictionary, value);
            }
        }

        /// <summary>
        /// Slide in help dictionary
        /// </summary>
        public static List<SlideInHelpDictionaryItem> SlideInHelpDictionary
        {
            get
            {
                return StateManager.GetApplication(EzCMSContants.SlideInHelpDictionary, new List<SlideInHelpDictionaryItem>());
            }
            set
            {
                StateManager.SetApplication(EzCMSContants.SlideInHelpDictionary, value);
            }
        }

        /// <summary>
        /// ref http://stackoverflow.com/questions/15141338/embed-git-commit-hash-in-a-net-dll
        /// </summary>
        public static string EzCMSVersion
        {
            get { return WorkContextHelper.GetEzCMSVersion(); }
            set
            {
                StateManager.SetApplication(EzCMSContants.EzCMSVersion, value);
            }
        }

        /// <summary>
        /// Check if site is live site or test site
        /// </summary>
        public static bool EnableLiveSiteMode
        {
            get
            {
                var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
                return siteSettingService.GetSetting<bool>(SettingNames.EnableLiveSiteMode);
            }
        }

        #endregion

        #region Setup

        /// <summary>
        /// Get site setup information
        /// </summary>
        public static SetupInformationCollection SetupInformation
        {
            get
            {
                return StateManager.GetSession(EzCMSContants.SetupSessionKey, new SetupInformationCollection());
            }
        }

        #endregion

        #region RazorEngine

        //TODO: change list to use Concurent Collection http://www.codeproject.com/Articles/181410/Thread-safe-Collections-in-NET
        /// <summary>
        /// Cache templates
        /// </summary>
        public static List<TemplateCache> TemplateCaches
        {
            get
            {
                return StateManager.GetApplication(EzCMSContants.TemplateCaches, new List<TemplateCache>(), true);
            }
            set
            {
                StateManager.SetApplication(EzCMSContants.TemplateCaches, value);
            }
        }

        #endregion

        #region User information

        /// <summary>
        /// Get current user stored in cuture
        /// </summary>
        public static UserSessionModel CurrentUser
        {
            get
            {
                return StateManager.GetSession<UserSessionModel>(EzCMSContants.CurrentUser);
            }
            set
            {
                StateManager.SetSession(EzCMSContants.CurrentUser, value);
            }
        }

        /// <summary>
        /// Current contact information
        /// </summary>
        public static ContactCookieModel CurrentContact
        {
            get { return WorkContextHelper.GetCurrentContact(); }
            set
            {
                WorkContextHelper.SetCurrentContact(value);
            }
        }

        #region Culture

        /// <summary>
        /// Get current session cuture
        /// </summary>
        public static string CurrentCulture
        {
            get { return WorkContextHelper.GetCurrentCulture(); }
            set
            {
                WorkContextHelper.SetCurrentCulture(value);
            }
        }

        /// <summary>
        /// Get available resouce managers
        /// </summary>
        public static List<ResourceManager> AvailableResourceManagers
        {
            get { return WorkContextHelper.GetAvailableResourceManagers(); }
        }

        /// <summary>
        /// Get current resource managers
        /// </summary>
        public static List<ResourceManager> CurrentResourceManagers
        {
            get { return WorkContextHelper.GetCurrentResourceManager(CurrentCulture); }
        }

        #endregion

        #region Timezone

        /// <summary>
        /// Current timezone cookie
        /// </summary>
        public static string CurrentTimezoneCookie
        {
            get { return WorkContextHelper.GetCurrentTimezoneCookie(CurrentUser); }
            set
            {
                StateManager.SetCookie(EzCMSContants.CurrentTimezone, value, DateTime.UtcNow.AddYears(1));
            }
        }

        /// <summary>
        /// Current timezone
        /// </summary>
        public static TimeZoneInfo CurrentTimezone
        {
            get
            {
                return CurrentTimezoneCookie.OlsonTimeZoneToTimeZoneInfo();
            }
        }

        /// <summary>
        /// Timezone offset of current user
        /// </summary>
        public static int TimezoneOffset
        {
            get
            {
                return CurrentTimezone == null ? 0 : (int)CurrentTimezone.GetUtcOffset(DateTime.UtcNow).TotalMilliseconds;
            }
        }

        #endregion

        #endregion

        #region Analytics

        /// <summary>
        /// Get Google analytic access token
        /// </summary>
        public static string GoogleAnalyticAccessToken
        {
            get
            {
                return StateManager.GetApplication<string>(EzCMSContants.GoogleAnalyticAccessToken);
            }
            set
            {
                StateManager.SetApplication(EzCMSContants.GoogleAnalyticAccessToken, value);
            }
        }

        #endregion

        #region Settings


        /// <summary>
        /// Site Settings
        /// </summary>
        public static List<SiteSettingModel> EzCMSSettings
        {
            get
            {
                return StateManager.GetApplication(FrameworkConstants.EzSettings, new List<SiteSettingModel>());
            }
            set
            {
                StateManager.SetApplication(FrameworkConstants.EzSettings, value);
            }
        }

        #endregion
    }
}