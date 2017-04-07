using Ez.Framework.Core.BackgroundTasks;
using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Logging;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Logging;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Framework.Mvc.ModelBinders;
using EzCMS.Core.Framework.Mvc.ViewEngines;
using EzCMS.Core.Framework.Mvc.ViewEngines.Razor.RazorEngine;
using EzCMS.Core.Framework.Mvc.ViewEngines.Razor.RazorEngine.Configs;
using EzCMS.Core.Framework.Utilities;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.PageTemplates;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Core.Services.SiteSetup;
using EzCMS.Core.Services.SlideInHelps;
using EzCMS.Core.Services.StarterKits;
using EzCMS.Core.Services.Widgets;
using EzCMS.Entity.Core.SiteInitialize;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Repositories.ClientNavigations;
using EzCMS.Entity.Repositories.Pages;
using EzCMS.Entity.Repositories.Users;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace EzCMS.Web
{
    public static class EzCMSConfig
    {
        /// <summary>
        /// Load plugin assemblies and build to AppDomain
        /// </summary>
        public static void RegisterPluginAssemblies()
        {
            var assemblies = EzCMSUtilities.GetEzReferencedAssemblies();

            foreach (var assemblyName in assemblies)
            {
                Assembly.Load(assemblyName);
            }

            if (SiteInitializer.IsSetupFinish())
            {
                var loadedPlugins = new List<Assembly>();
                var pluginDllPaths = PluginHelper.LoadAllInstalledPluginDlls();
                foreach (var pluginDllPath in pluginDllPaths)
                {
                    try
                    {
                        loadedPlugins.Add(Assembly.LoadFile(pluginDllPath));
                    }
                    catch (FileLoadException)
                    {
                    }
                    catch (BadImageFormatException)
                    {
                    }
                }
                loadedPlugins.ForEach(BuildManager.AddReferencedAssembly);
            }
        }

        #region Register Process

        /// <summary>
        /// Register all required processes
        /// </summary>
        public static void RegisterProcess()
        {
            //Load localize resources
            RegisterLocalizedResources();

            //Load slide in help resources
            RegisterSlideInHelpResources();

            //Initialize site settings
            RegisterSiteSettings();

            //Initialize widgets
            RegisterWidgets();

            //Initialize page template cache
            RegisterPageTemplates();
        }

        #region Processes

        /// <summary>
        /// Initialize localized resources
        /// </summary>
        private static void RegisterLocalizedResources()
        {
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            localizedResourceService.RefreshDictionary();
        }

        /// <summary>
        /// Initialize slide in help resources
        /// </summary>
        private static void RegisterSlideInHelpResources()
        {
            var slideInHelpService = HostContainer.GetInstance<ISlideInHelpService>();
            slideInHelpService.RefreshDictionary();
        }

        /// <summary>
        /// Initialize Site Settings
        /// </summary>
        private static void RegisterSiteSettings()
        {
            var settingService = HostContainer.GetInstance<ISiteSettingService>();
            settingService.Initialize();
        }

        /// <summary>
        /// Initialize Widgets
        /// </summary>
        private static void RegisterWidgets()
        {
            var widgetService = HostContainer.GetInstance<IWidgetService>();
            WorkContext.Widgets = widgetService.GetAllWidgets(false);
        }

        /// <summary>
        /// Initialize cache layout for page templates
        /// </summary>
        private static void RegisterPageTemplates()
        {
            var pageTemplateService = HostContainer.GetInstance<IPageTemplateService>();
            pageTemplateService.Initialize();
        }

        #endregion

        #endregion

        /// <summary>
        /// Register the view engine for application
        /// </summary>
        public static void RegisterViewEngine()
        {
            //Remove all engines
            ViewEngines.Engines.Clear();

            #region RazorEngine

            var config = new TemplateServiceConfiguration
            {
                BaseTemplateType = typeof(RazorEngineTemplateBase<>),
                EncodedStringFactory = new RazorEngineMvcHtmlStringFactory(),
                TemplateManager = new RazorEngineTemplateManager(),
                Debug = false
            };

            #region Namespaces

            var webConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web.config");
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = webConfigPath };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var razorConfig = configuration.GetSection("system.web/pages") as PagesSection;

            if (razorConfig != null)
            {
                foreach (NamespaceInfo namespaceInfo in razorConfig.Namespaces)
                {
                    config.Namespaces.Add(namespaceInfo.Namespace);
                }
            }

            #endregion

            Engine.Razor = RazorEngineService.Create(config);

            #endregion

            //Add Razor Engine
            ViewEngines.Engines.Add(new EzCMSRazorEngine());

            HostingEnvironment.RegisterVirtualPathProvider(new EzCMSVirtualPathProvider());
        }

        #region Simple Injector

        #region Register Application

        /// <summary>
        /// Simple injector initialize
        /// </summary>
        public static void RegisterIOC()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            RegisterAllDependencies(container);

            // Set the service locator here
            HostContainer.SetContainer(container);
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            // Verify the container
            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }

        /// <summary>
        /// Initialize all required interface
        /// </summary>
        /// <param name="container"></param>
        private static void RegisterAllDependencies(Container container)
        {
            container.Register(() => new EzCMSEntities(), Lifestyle.Scoped);
            container.Register<DbContext>(() => new EzCMSEntities(), Lifestyle.Scoped);

            container.Register(typeof(IRepository<>), typeof(Repository<>), Lifestyle.Scoped);
            container.Register(typeof(IHierarchyRepository<>), typeof(HierarchyRepository<>), Lifestyle.Scoped);

            IoCRegister.RegisterDepencies(container);

            // Logger using site settings
            container.Register(typeof(ILogger), () => new Logger(MethodBase.GetCurrentMethod().DeclaringType), Lifestyle.Scoped);

            #region Register controllers

            //container.RegisterMvcControllers(EzCMSUtilities.GetEzCMSAssemblies().ToArray());

            var registeredControllerTypes = SimpleInjectorMvcExtensions.GetControllerTypesToRegister(
                container, EzCMSUtilities.GetEzCMSAssemblies().ToArray());

            // Remove setup controller out of Register process
            registeredControllerTypes = registeredControllerTypes.Where(type => type.Name != "SetupController").ToArray();

            foreach (var controllerType in registeredControllerTypes)
            {
                RegisterController(container, controllerType);
            }

            #endregion
        }

        #endregion

        #region Register For Setup Process

        /// <summary>
        /// Register setup service for setup process
        /// </summary>
        public static void RegisterSetupIOC()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            #region Register required service for setup process

            // Register site setup service
            container.Register(() => new EzCMSEntities(), Lifestyle.Scoped);
            container.Register<DbContext>(() => new EzCMSEntities(), Lifestyle.Scoped);

            container.Register(typeof(ILogger), () => new Logger(MethodBase.GetCurrentMethod().DeclaringType));
            container.Register(typeof(IRepository<>), typeof(Repository<>));
            container.Register(typeof(IHierarchyRepository<>), typeof(HierarchyRepository<>));

            container.Register(typeof(IPageRepository), typeof(PageRepository));
            container.Register(typeof(IUserRepository), typeof(UserRepository));
            container.Register(typeof(IClientNavigationRepository), typeof(ClientNavigationRepository));
            container.Register(typeof(ISiteSetupService), typeof(SiteSetupService));
            container.Register(typeof(IStarterKitService), typeof(StarterKitService));
            container.Register(typeof(IBackgroundTaskManager), typeof(BackgroundTaskManager));

            #endregion

            #region Register SetupController

            // Only get SetupController
            var registeredControllerTypes = SimpleInjectorMvcExtensions.GetControllerTypesToRegister(
                container, EzCMSUtilities.GetEzCMSAssemblies().ToArray()).Where(type => type.Name == "SetupController");

            foreach (var controllerType in registeredControllerTypes)
            {
                RegisterController(container, controllerType);
            }

            #endregion

            HostContainer.SetContainer(container);
            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }

        private static void RegisterController(Container container, Type controllerType)
        {
            Registration registration = Lifestyle.Transient.CreateRegistration(controllerType, container);
            registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "MVC's DefaultControllerFactory disposes the controller when the web request ends.");
            container.AddRegistration(controllerType, registration);
        }

        #endregion

        #endregion

        #region Routing

        /// <summary>
        /// Register area
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="routes"></param>
        /// <param name="state"></param>
        public static void RegisterArea<T>(RouteCollection routes, object state) where T : AreaRegistration
        {
            var registration = (AreaRegistration)Activator.CreateInstance(typeof(T));
            var registrationContext = new AreaRegistrationContext(registration.AreaName, routes, state);
            string areaNamespace = registration.GetType().Namespace;
            if (!string.IsNullOrEmpty(areaNamespace))
                registrationContext.Namespaces.Add(areaNamespace + ".*");
            registration.RegisterArea(registrationContext);
        }

        //public void RegisterAllDependencies()
        //{
        //    var routes = HostContainer.GetInstances<IEzCMSRouteRegister>().Distinct();
        //    foreach (var EzCMSRouteRegister in routes)
        //    {
        //        try
        //        {
        //            Type T = EzCMSRouteRegister.GetType();
        //            var registration = (AreaRegistration)Activator.CreateInstance(T);
        //            var registrationContext = new AreaRegistrationContext(registration.AreaName, RouteTable.Routes, null);
        //            string areaNamespace = registration.GetType().Namespace;
        //            if (!string.IsNullOrEmpty(areaNamespace))
        //                registrationContext.Namespaces.Add(areaNamespace + ".*");
        //            registration.RegisterArea(registrationContext);
        //        }
        //        catch { }
        //    }
        //}

        #endregion

        #region Model Binders

        public static void RegisterModelBinders()
        {
            // Register datetime model binder for time zone convert
            var dateTimeModelBinder = new DateTimeModelBinder();
            ModelBinders.Binders.Add(typeof(DateTime), dateTimeModelBinder);
            ModelBinders.Binders.Add(typeof(DateTime?), dateTimeModelBinder);

            // Register timespan model binder for time zone convert
            var timeSpanModelBinder = new TimeSpanModelBinder();
            ModelBinders.Binders.Add(typeof(TimeSpan), timeSpanModelBinder);
            ModelBinders.Binders.Add(typeof(TimeSpan?), timeSpanModelBinder);
        }

        #endregion
    }
}