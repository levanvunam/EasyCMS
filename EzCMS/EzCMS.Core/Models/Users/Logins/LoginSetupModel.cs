using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Enums;
using System.Linq;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.SiteSettings;

namespace EzCMS.Core.Models.Users.Logins
{
    public class LoginSetupModel
    {
        public LoginSetupModel()
        {

        }

        public LoginSetupModel(LoginEnums.LoginTemplateConfiguration templateConfiguration)
            : this()
        {
            var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
            var loginTemplateSetting = siteSettingService.LoadSetting<LoginTemplateSetting>();

            var config =
                loginTemplateSetting.Configurations.FirstOrDefault(
                    t => t.LoginTemplateConfiguration == templateConfiguration);

            if (config != null)
            {
                Enable = config.Enable;
                EnableTemplate = config.EnableTemplate;
                Template = config.Template;
                TemplateName = config.LoginTemplateConfiguration.GetEnumName();
            }
        }

        #region Public Properties

        public bool Enable { get; set; }

        public bool EnableTemplate { get; set; }

        public string Template { get; set; }

        public string TemplateName { get; set; }

        #endregion
    }
}