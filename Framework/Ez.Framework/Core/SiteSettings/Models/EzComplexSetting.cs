using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.Entity.Intialize;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.SiteSettings.Services;
using Ez.Framework.Utilities;
using System.Data.Entity;

namespace Ez.Framework.Core.SiteSettings.Models
{
    public abstract class EzComplexSetting : IComplexSetting
    {
        #region Setup

        /// <summary>
        /// Setup data initialize
        /// </summary>
        /// <returns></returns>
        public virtual DataInitializerPriority Priority()
        {
            return DataInitializerPriority.Low;
        }

        /// <summary>
        /// Initialize setting
        /// </summary>
        /// <param name="ezDbContext"></param>
        public abstract void Initialize(DbContext ezDbContext);

        /// <summary>
        /// Get site setting setup
        /// </summary>
        /// <returns></returns>
        public abstract ComplexSettingSetup GetSetup();

        #endregion

        #region Load setting

        /// <summary>
        /// Load setting
        /// </summary>
        /// <returns></returns>
        public virtual T LoadSetting<T>()
        {
            var settingService = HostContainer.GetInstance<IEzSiteSettingService>();
            var settingString = settingService.GetSetting<string>(GetSetup().Name);

            if (string.IsNullOrEmpty(settingString))
            {
                var setting = GetSetup().DefaultValue;
                settingString = SerializeUtilities.Serialize(setting);
                settingService.InsertSetting(new SiteSetting
                {
                    Name = GetSetup().Name,
                    Description = GetSetup().Description,
                    Value = settingString,
                    SettingType = GetSetup().SettingType
                });
                return setting;
            }

            return SerializeUtilities.Deserialize<T>(settingString);
        }

        #endregion
    }
}