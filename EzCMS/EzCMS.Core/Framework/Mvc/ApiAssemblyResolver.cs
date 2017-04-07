using Ez.Framework.Configurations;
using Ez.Framework.Utilities.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace EzCMS.Core.Framework.Mvc
{
    /// <summary>
    /// Api assembly resolver
    /// </summary>
    public class ApiAssemblyResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            var assemblies = ReflectionUtilities.GetAssemblies(FrameworkConstants.EzSolution).ToList();

            return assemblies;
        }
    }
}