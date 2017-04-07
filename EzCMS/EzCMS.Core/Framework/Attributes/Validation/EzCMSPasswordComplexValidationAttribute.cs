using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Core.IoC;
using Ez.Framework.IoC;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Services.SiteSettings;

namespace EzCMS.Core.Framework.Attributes.Validation
{
    /// <summary>
    /// Defines the <see cref="EzCMSPasswordComplexValidationAttribute" /> object
    /// </summary>
    public class EzCMSPasswordComplexValidationAttribute : PasswordComplexValidationAttribute
    {
        public EzCMSPasswordComplexValidationAttribute()
        {
            var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
            var passwordSetting = siteSettingService.LoadSetting<PasswordSetting>();

            if (passwordSetting != null)
            {
                Config(passwordSetting.PasswordMinLengthRequired, passwordSetting.PasswordMustHaveDigit,
                    passwordSetting.PasswordMustHaveSymbol, passwordSetting.PasswordMustHaveUpperAndLowerCaseLetters);
            }
        }
    }
}
