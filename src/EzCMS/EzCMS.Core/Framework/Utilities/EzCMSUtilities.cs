using Ez.Framework.Configurations;
using Ez.Framework.Utilities.Reflection;
using System.Collections.Generic;
using System.Reflection;

namespace EzCMS.Core.Framework.Utilities
{
    public static class EzCMSUtilities
    {
        #region Ez Assembly Helpers

        /// <summary>
        /// Get Ez assemblies
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetEzCMSAssemblies()
        {
            return ReflectionUtilities.GetAssemblies(FrameworkConstants.EzSolution);
        }

        /// <summary>
        /// Get Ez assemblies with prefix
        /// </summary>
        /// <returns></returns>
        public static List<AssemblyName> GetEzReferencedAssemblies()
        {
            return ReflectionUtilities.GetReferencedAssemblies(FrameworkConstants.EzSolution);
        }

        #endregion
    }
}
