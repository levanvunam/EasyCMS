using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.SiteSettings.Models;

namespace Ez.Framework.Core.SiteSettings.Services
{
    [Register(Lifetime.PerInstance)]
    public interface IEzSiteSettingService
    {
        #region Initialize

        /// <summary>
        /// Initialize site settings
        /// </summary>
        void Initialize();

        #endregion

        /// <summary>
        /// Insert setting
        /// </summary>
        /// <param name="siteSetting"></param>
        /// <returns></returns>
        ResponseModel InsertSetting(SiteSetting siteSetting);

        /// <summary>
        /// Update setting
        /// </summary>
        /// <param name="siteSetting"></param>
        /// <returns></returns>
        ResponseModel UpdateSetting(SiteSetting siteSetting);

        /// <summary>
        /// Get setting by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetSetting<T>(int id);

        /// <summary>
        /// Load setting by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetSetting<T>(string key);

        /// <summary>
        /// Load setting by object type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T LoadSetting<T>() where T : EzComplexSetting;
    }
}
