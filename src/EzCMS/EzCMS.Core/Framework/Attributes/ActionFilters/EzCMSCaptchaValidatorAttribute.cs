using Ez.Framework.Core.Attributes.ActionFilters;
using Ez.Framework.Core.IoC;
using Ez.Framework.IoC;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Services.SiteSettings;

namespace EzCMS.Core.Framework.Attributes.ActionFilters
{
    public class EzCMSCaptchaValidatorAttribute : CaptchaValidatorAttribute
    {
        /// <summary>
        /// Name of captcha to validation
        /// </summary>
        public string Name { get; set; }

        #region Contructors

        public EzCMSCaptchaValidatorAttribute()
        {
        }

        public EzCMSCaptchaValidatorAttribute(string name)
            : this()
        {
            Name = name;

            var settingService = HostContainer.GetInstance<ISiteSettingService>();
            var captchaSetting = settingService.LoadSetting<CaptchaSetting>();
            if (!string.IsNullOrEmpty(captchaSetting.PublicKey) && !string.IsNullOrEmpty(captchaSetting.PrivateKey))
            {
                SetKeys(captchaSetting.PrivateKey, captchaSetting.PublicKey);
            }
        }

        #endregion
    }
}
