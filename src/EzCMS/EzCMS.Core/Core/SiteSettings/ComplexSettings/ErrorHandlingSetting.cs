using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.SiteSettings;
using System.ComponentModel;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class ErrorHandlingSetting : ComplexSetting
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
                Name = SettingNames.ErrorHandlingConfiguration,
                SettingType = "system",
                DefaultValue = new ErrorHandlingSetting
                {
                    EmailFrom = EzCMSContants.EasyCMSEmail,
                    EmailTo = EzCMSContants.NotificationEmail,
                    LoggingOption = LoggingOption.LogAndSendEmail,
                    PageNotFoundUrl = "/Page-Not-Found",
                    PageErrorUrl = "/Page-Error"
                }
            };
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Page not found url
        /// </summary>
        [LocalizedDisplayName("SiteSetting_ErrorHandlingSetting_Field_PageNotFoundUrl")]
        public string PageNotFoundUrl { get; set; }

        /// <summary>
        /// Page error url
        /// </summary>
        [LocalizedDisplayName("SiteSetting_ErrorHandlingSetting_Field_PageErrorUrl")]
        public string PageErrorUrl { get; set; }

        /// <summary>
        /// Email from
        /// </summary>
        [LocalizedDisplayName("SiteSetting_ErrorHandlingSetting_Field_EmailFrom")]
        public string EmailFrom { get; set; }

        /// <summary>
        /// Email to
        /// </summary>
        [LocalizedDisplayName("SiteSetting_ErrorHandlingSetting_Field_EmailTo")]
        public string EmailTo { get; set; }

        /// <summary>
        /// Logging option
        /// </summary>
        [LocalizedDisplayName("SiteSetting_ErrorHandlingSetting_Field_LoggingOption")]
        public LoggingOption LoggingOption { get; set; }

        #endregion
    }

    public enum LoggingOption
    {
        [Description("Log & Send Email")]
        LogAndSendEmail = 1,

        [Description("Log Only")]
        LogOnly = 2,

        [Description("Send Email Only")]
        SendEmailOnly = 3,

        [Description("Do Nothing")]
        DoNothing = 4
    }
}