using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.SiteSettings;
using System.Collections.Generic;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class AccountExpiredSetting : ComplexSetting
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
                Name = SettingNames.AccountExpiredSetting,
                SettingType = "system",
                DefaultValue = new AccountExpiredSetting
                {
                    NumberOfDaysToKeepAccountAlive = 90,
                    DaysPriorToSuspensionShouldNotify = 7
                }
            };
        }

        #endregion

        #region Public Properties

        [GreaterThan("DaysPriorToSuspensionShouldNotify")]
        [RequiredInteger]
        [LocalizedDisplayName("SiteSetting_AccountExpiredSetting_Field_NumberOfDaysToKeepAccountAlive")]
        public int NumberOfDaysToKeepAccountAlive { get; set; }

        [RequiredInteger]
        [LocalizedDisplayName("SiteSetting_AccountExpiredSetting_Field_DaysPriorToSuspensionShouldNotify")]
        public int DaysPriorToSuspensionShouldNotify { get; set; }

        [LocalizedDisplayName("SiteSetting_AccountExpiredSetting_Field_ExpirableUserGroupIds")]
        public List<int> ExpirableUserGroupIds { get; set; }

        #endregion
    }
}