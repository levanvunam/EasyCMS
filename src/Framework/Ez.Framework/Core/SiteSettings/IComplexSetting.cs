using Ez.Framework.Core.Entity.Intialize;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.SiteSettings.Models;

namespace Ez.Framework.Core.SiteSettings
{
    [Register(Lifetime.PerInstance)]
    public interface IComplexSetting : IDataInitializer
    {
        /// <summary>
        /// Get setting setup
        /// </summary>
        /// <returns></returns>
        ComplexSettingSetup GetSetup();

        /// <summary>
        /// Load setting
        /// </summary>
        /// <returns></returns>
        T LoadSetting<T>();
    }
}
