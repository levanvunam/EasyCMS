using Ez.Framework.Utilities;
using EzCMS.Core.Framework.EmbeddedResource;

namespace EzCMS.Core.Framework.StarterKits
{
    public class StarterKitHelper
    {
        private const string Namespace = "EzCMS.Core.Core.StarterKits";

        public static string GetEzCMSResource(string name, string template, ResourceTypeEnums resourceType)
        {
            var @namespace = string.Format("{0}.{1}.Resources.{2}", Namespace, template, resourceType.GetEnumName());
            return EmbeddedResourceHelper.GetString(name, @namespace);
        }

        /// <summary>
        /// Get starter 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public static string GetResource(string name, string @namespace)
        {
            return EmbeddedResourceHelper.GetString(name, @namespace);
        }
    }
}
