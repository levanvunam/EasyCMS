using Ez.Framework.Configurations;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.IoC.IDependencyRegisters;
using Ez.Framework.IoC.Attributes;
using Ez.Framework.Utilities.Reflection;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using System;
using System.Linq;
using System.Reflection;

namespace Ez.Framework.Core.IoC
{
    public class IoCRegister
    {
        public static void RegisterDepencies(Container containter)
        {
            var assemblies = ReflectionUtilities.GetAssemblies(FrameworkConstants.EzSolution).ToList();
            var dependencyInterfaces = assemblies.SelectMany(i => i.ExportedTypes).Where(i => i.IsInterface && i.GetCustomAttribute<RegisterAttribute>() != null);
            foreach (var dependencyInterface in dependencyInterfaces)
            {
                var registerAttr = dependencyInterface.GetCustomAttribute<RegisterAttribute>();

                // Get all implementations of RegisterAttribute
                var implementations = dependencyInterface.GetAllImplementTypes(FrameworkConstants.EzSolution).ToList();
                Lifestyle lifestyle = Lifestyle.Transient;

                switch (registerAttr.ScopeLifetime)
                {
                    case Lifetime.PerInstance:
                        lifestyle = Lifestyle.Transient;
                        break;
                    case Lifetime.PerRequest:
                        lifestyle = new WebRequestLifestyle();
                        break;
                    case Lifetime.SingleTon:
                        lifestyle = Lifestyle.Singleton;
                        break;
                }

                if (implementations.Count == 1)
                {
                    containter.Register(dependencyInterface, implementations.First(), lifestyle);
                }
                else
                {
                    var registrations = implementations.Select(i => lifestyle.CreateRegistration(i, containter));
                    containter.RegisterCollection(dependencyInterface, registrations);
                }
            }

            #region Module Registers

            // Register generic dependencies first
            var dependencyRegisters = typeof(IDependencyRegister).GetAllImplementTypesOf(FrameworkConstants.EzSolution);
            foreach (var dependencyRegister in dependencyRegisters)
            {
                var implementaton = (IDependencyRegister)Activator.CreateInstance(dependencyRegister);
                implementaton.Register(containter);
            }

            // Register service
            dependencyInterfaces = assemblies.SelectMany(i => i.ExportedTypes).Where(i => i.IsInterface && i.GetCustomAttribute<ModuleRegisterAttribute>() != null);
            foreach (var dependencyInterface in dependencyInterfaces)
            {
                var registerAttr = dependencyInterface.GetCustomAttribute<ModuleRegisterAttribute>();

                // Get all implementations of RegisterAttribute
                var implementations = dependencyInterface.GetAllImplementTypes(FrameworkConstants.EzSolution).ToList();
                Lifestyle lifestyle = Lifestyle.Transient;

                switch (registerAttr.ScopeLifetime)
                {
                    case Lifetime.PerInstance:
                        lifestyle = Lifestyle.Transient;
                        break;
                    case Lifetime.PerRequest:
                        lifestyle = new WebRequestLifestyle();
                        break;
                    case Lifetime.SingleTon:
                        lifestyle = Lifestyle.Singleton;
                        break;
                }

                if (implementations.Count == 1)
                {
                    containter.Register(dependencyInterface, implementations.First(), lifestyle);
                }
                else
                {
                    var registrations = implementations.Select(i => lifestyle.CreateRegistration(i, containter));
                    containter.RegisterCollection(dependencyInterface, registrations);
                }
            }

            #endregion
        }
    }
}
