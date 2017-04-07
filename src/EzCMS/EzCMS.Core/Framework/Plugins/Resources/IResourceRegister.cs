using Ez.Framework.Core.IoC.Attributes;
using System.Collections.Generic;
using System.Resources;

namespace EzCMS.Core.Framework.Plugins.Resources
{

    [Register(Lifetime.SingleTon)]
    public interface IResourceRegister
    {
        /// <summary>
        /// Register all plugin resources
        /// </summary>
        /// <returns></returns>
        List<ResourceManager> RegisterResources();
    }
}
