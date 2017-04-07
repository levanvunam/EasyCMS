using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;
using System;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class EventConfiguration : ComplexSetting
    {
        #region Setup

        /// <summary>
        /// Get complex setting setup
        /// </summary>
        /// <returns></returns>
        public override ComplexSettingSetup GetSetup()
        {
            return new ComplexSettingSetup
            {
                Name = SettingNames.EventConfiguration,
                SettingType = "system",
                DefaultValue = new EventConfiguration
                {
                    DefaultStartHour = null,
                    DefaultEndHour = null
                }
            };
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SiteSetting_EventSetting_Field_DefaultStartHour")]
        public DateTime? DefaultStartHour { get; set; }

        [LocalizedDisplayName("SiteSetting_EventSetting_Field_DefaultEndHour")]
        public DateTime? DefaultEndHour { get; set; }

        #endregion
    }
}
