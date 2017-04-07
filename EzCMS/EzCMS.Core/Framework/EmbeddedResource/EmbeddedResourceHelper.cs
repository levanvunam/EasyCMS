using EzCMS.Core.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EzCMS.Core.Framework.EmbeddedResource
{
    public static class EmbeddedResourceHelper
    {
        /// <summary>
        /// Get embedded resource by short name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The resource as stream if there is only 1 resource whose fullname ends with provided name. Null if no match. Throws Argument Exception if there are more than 1 matches</returns>
        public static Stream GetStream(string name)
        {
            var assemblies = EzCMSUtilities.GetEzCMSAssemblies();
            foreach (var assembly in assemblies)
            {
                //Skip dynamic assemblies
                if (assembly.IsDynamic)
                {
                    continue;
                }
                var resourceNames = assembly.GetManifestResourceNames().Where(r => r.Equals(name)).ToList();
                if (resourceNames.Count > 1)
                {
                    throw new ArgumentException(string.Format("Ambigous resource name: {0}", resourceNames.First()));
                }
                if (resourceNames.Any())
                {
                    return assembly.GetManifestResourceStream(resourceNames.First());
                }
            }
            throw new ArgumentException(string.Format("Missing resource name: {0}", name));
        }

        /// <summary>
        /// Get embedded resource by short name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="namespace"></param>
        /// <param name="assembly">The assembly to look for resource, the calling assembly will be use if null is passed</param>
        /// <returns>The resource as string if there is only 1 resource whose fullname ends with provided name. Null if no match. Throws Argument Exception if there are more than 1 matches</returns>
        public static string GetString(string name, string @namespace, Assembly assembly = null)
        {
            if (!string.IsNullOrEmpty(@namespace))
            {
                name = string.Format("{0}.{1}", @namespace, name);
            }
            var stream = GetStream(name);
            using (var reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }

        /// <summary>
        /// Get embedded resource by short name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="namespace"></param>
        /// <param name="assembly">The assembly to look for resource, the calling assembly will be use if null is passed</param>
        /// <returns>The resource as string if there is only 1 resource whose fullname ends with provided name. Null if no match. Throws Argument Exception if there are more than 1 matches</returns>
        public static string GetNullableString(string name, string @namespace, Assembly assembly = null)
        {
            try
            {
                return GetString(name, @namespace, assembly);
            }
            catch (Exception)
            {
                //If cannot find the resource then return empty string
                return string.Empty;
            }
        }

        #region Folder

        /// <summary>
        /// Get all embedded resource of folder.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="namespace"></param>
        /// <param name="assembly">The assembly to look for resource, the calling assembly will be use if null is passed</param>
        /// <returns>The resource as string if there is only 1 resource whose fullname ends with provided name. Null if no match. Throws Argument Exception if there are more than 1 matches</returns>
        public static List<EmbeddedResource> GetEmbeddedResourcesOfFolder(string folder, string @namespace, Assembly assembly = null)
        {
            if (!string.IsNullOrEmpty(@namespace))
            {
                folder = string.Format("{0}.{1}", @namespace, folder);
            }
            var embeddedResources = GetEmbeddedResourcesOfFolder(folder);

            foreach (var embeddedResource in embeddedResources)
            {
                using (var reader = new StreamReader(embeddedResource.Stream))
                {
                    string result = reader.ReadToEnd();
                    embeddedResource.Content = result;
                }
            }

            return embeddedResources;
        }

        /// <summary>
        /// Get embedded resource by short name.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns>The resource as stream if there is only 1 resource whose fullname ends with provided name. Null if no match. Throws Argument Exception if there are more than 1 matches</returns>
        public static List<EmbeddedResource> GetEmbeddedResourcesOfFolder(string folder)
        {
            var assemblies = EzCMSUtilities.GetEzCMSAssemblies();
            foreach (var assembly in assemblies)
            {
                //Skip dynamic assemblies
                if (assembly.IsDynamic)
                {
                    continue;
                }
                var resourceNames = assembly.GetManifestResourceNames().Where(r => r.StartsWith(folder)).ToList();

                if (resourceNames.Any())
                {
                    return resourceNames.Select(r => new EmbeddedResource
                    {
                        Name = r.Replace(folder + ".", ""),
                        Namespace = r,
                        Stream = assembly.GetManifestResourceStream(r)
                    }).ToList();
                }
            }
            throw new ArgumentException(string.Format("Missing resource folder: {0}", folder));
        }

        /// <summary>
        /// Get all embedded resource names of folder.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="namespace"></param>
        /// <param name="assembly">The assembly to look for resource, the calling assembly will be use if null is passed</param>
        /// <returns>The resource as string if there is only 1 resource whose fullname ends with provided name. Null if no match. Throws Argument Exception if there are more than 1 matches</returns>
        public static List<string> GetEmbeddedResourceNamesOfFolder(string folder, string @namespace, Assembly assembly = null)
        {
            if (!string.IsNullOrEmpty(@namespace))
            {
                folder = string.Format("{0}.{1}", @namespace, folder);
            }

            return GetEmbeddedResourceNamesOfFolder(folder);
        }

        /// <summary>
        /// Get embedded resource by short name.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns>The resource as stream if there is only 1 resource whose fullname ends with provided name. Null if no match. Throws Argument Exception if there are more than 1 matches</returns>
        public static List<string> GetEmbeddedResourceNamesOfFolder(string folder)
        {
            var assemblies = EzCMSUtilities.GetEzCMSAssemblies();
            List<string> embeddedResources = new List<string>();
            foreach (var assembly in assemblies)
            {
                //Skip dynamic assemblies
                if (assembly.IsDynamic)
                {
                    continue;
                }
                embeddedResources.AddRange(assembly.GetManifestResourceNames().Where(r => r.StartsWith(folder)).Select(r => r).ToList());

            }

            return embeddedResources;
        }

        #endregion
    }

    public class EmbeddedResource
    {
        public string Namespace { get; set; }

        public string Name { get; set; }

        public Stream Stream { get; set; }

        public string Content { get; set; }
    }
}