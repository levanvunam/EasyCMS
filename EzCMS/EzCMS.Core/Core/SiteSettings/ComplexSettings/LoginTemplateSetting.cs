using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Framework.SiteSettings;
using System;
using System.Collections.Generic;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class LoginTemplateSetting : ComplexSetting
    {
        #region Setup

        /// <summary>
        /// Get complex setting setup
        /// </summary>
        /// <returns></returns>
        public override ComplexSettingSetup GetSetup()
        {
            var configurations = EnumUtilities.GetAllItems<LoginEnums.LoginTemplateConfiguration>();

            var templates = new List<TemplateConfiguration>();
            foreach (var configuration in configurations)
            {
                var templateConfiguration = new TemplateConfiguration
                {
                    LoginTemplateConfiguration = configuration,
                    Enable = true,
                    EnableTemplate = false,
                    Template = string.Empty
                };

                try
                {
                    templateConfiguration.Template =
                        DataInitializeHelper.GetResourceContent(
                            string.Format("LoginTemplateSetting_{0}.cshtml", configuration),
                            DataSetupResourceType.SiteSetting);
                }
                catch (Exception)
                {
                    //Do nothing
                }

                templates.Add(templateConfiguration);
            }

            return new ComplexSettingSetup
            {
                Name = SettingNames.LoginTemplateSetting,
                SettingType = "system",
                DefaultValue = new LoginTemplateSetting
                {
                    Configurations = templates
                }
            };
        }

        #endregion

        #region Public Properties

        public List<TemplateConfiguration> Configurations { get; set; }

        #endregion
    }

    public class TemplateConfiguration
    {
        public LoginEnums.LoginTemplateConfiguration LoginTemplateConfiguration { get; set; }

        [LocalizedDisplayName("SiteSetting_LoginTemplateSetting_Field_Enable")]
        public bool Enable { get; set; }

        [LocalizedDisplayName("SiteSetting_LoginTemplateSetting_Field_EnableTemplate")]
        public bool EnableTemplate { get; set; }

        [LocalizedDisplayName("SiteSetting_LoginTemplateSetting_Field_Template")]
        public string Template { get; set; }
    }
}