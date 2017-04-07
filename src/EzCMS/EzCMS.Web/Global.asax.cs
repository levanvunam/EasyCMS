using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Mvc;
using EzCMS.Entity.Core.SiteInitialize;
using EzCMS.Entity.Entities;
using EzCMS.Web.Areas.BackgroundTasks.Models;
using EzCMS.Web.Areas.SiteSetup;
using System;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Configuration = EzCMS.Entity.Migrations.Configuration;

namespace EzCMS.Web
{
    public class MvcApplication : HttpApplication
    {
        private static IBackgroundTaskManager _backgroundTaskManager;

        /// <summary>
        /// Application start
        /// </summary>
        protected void Application_Start()
        {
            RouteTable.Routes.Clear();

            //Check if site is setup, if not just register route to the setup controller
            if (!SiteInitializer.IsSetupFinish())
            {
                //Initialize setup ioc
                EzCMSConfig.RegisterSetupIOC();

                EzCMSConfig.RegisterArea<SiteSetupAreaRegistration>(RouteTable.Routes, null);

                //Do not start background task if site is not set up.
                //assign non-null value to backgroundTaskManager will no trigger to start BackgroundTaskManager in Begin_Request
                _backgroundTaskManager = HostContainer.GetInstance<IBackgroundTaskManager>();
                return;
            }

            // Config log4net
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/log4net.config")));

            // Set database initializer only if site is finish setup 
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EzCMSEntities, Configuration>());

            //Set assemblies resolver for api register
            GlobalConfiguration.Configuration.Services.Replace(typeof(IAssembliesResolver), new ApiAssemblyResolver());

            //Initialize Simple Injector
            EzCMSConfig.RegisterIOC();

            //Register View Engine
            EzCMSConfig.RegisterViewEngine();

            //Register Model Binders
            EzCMSConfig.RegisterModelBinders();

            #region Routing

            // Register all areas will include the plugins as well
            AreaRegistration.RegisterAllAreas();

            // Register web api
            WebApiConfig.Register(GlobalConfiguration.Configuration);

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            #endregion

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Register some parts of core application
            EzCMSConfig.RegisterProcess();
        }

        /// <summary>
        /// Begin app request
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void Application_BeginRequest(Object source, EventArgs e)
        {
            try
            {
                var result = Interlocked.CompareExchange(ref _backgroundTaskManager,
                    HostContainer.GetInstance<IBackgroundTaskManager>(), null);
                if (result == null)
                {
                    _backgroundTaskManager.Start(typeof(ProxyTask));
                }
            }
            catch (Exception)
            {
                //DO NOTHING
            }
        }

        /// <summary>
        /// Set request state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            try
            {
                // Set culture by user settings
                var culture = WorkContext.CurrentCulture;

                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            }
            catch (Exception)
            {
                /* 
                 * Ignore because cannot get current culture because of some case:
                 *  - In setup process
                 *  - Error related to getting data
                */
            }
        }
    }
}