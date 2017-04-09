using System.Data.Entity;
using System.Reflection;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Logging;
using EzCMS.Core.Framework.Logging;
using EzCMS.Core.Framework.Utilities;
using EzCMS.Entity.Entities;
using SimpleInjector;
using SimpleInjector.Integration.Web;

namespace EzCMS.Core.Tests
{
    /// <summary>
    /// Test utilities
    /// </summary>
    public static class TestUtilties
    {
        /// <summary>
        /// Load all project assemblies
        /// </summary>
        public static void LoadProjectAssemblies()
        {
            var assemblies = EzCMSUtilities.GetEzReferencedAssemblies();

            foreach (var assemblyName in assemblies)
            {
                Assembly.Load(assemblyName);
            }
        }

        /// <summary>
        /// Register all IOC
        /// </summary>
        public static void RegisterIOC()
        {
            //Load project assemblies
            LoadProjectAssemblies();

            //Register IOC
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            container.Register(() => new EzCMSEntities(), Lifestyle.Scoped);
            container.Register<DbContext>(() => new EzCMSEntities(), Lifestyle.Scoped);

            container.Register(typeof(IRepository<>), typeof(Repository<>), Lifestyle.Scoped);
            container.Register(typeof(IHierarchyRepository<>), typeof(HierarchyRepository<>), Lifestyle.Scoped);
            IoCRegister.RegisterDepencies(container);

            // Logger using site settings
            container.Register(typeof(ILogger), () => new Logger(MethodBase.GetCurrentMethod().DeclaringType), Lifestyle.Scoped);
            HostContainer.SetContainer(container);
            container.Verify();
        }
    }
}