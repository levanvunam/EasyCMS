using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Framework.SiteSettings;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class FormBuilderSetting : ComplexSetting
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
                Name = SettingNames.FormBuilderSetting,
                SettingType = "system",
                DefaultValue = new FormBuilderSetting
                {
                    EmbeddedTemplate = DataInitializeHelper.GetResourceContent("FormBuilder.EmbeddedTemplate.html", DataSetupResourceType.SiteSetting),
                    RenderScriptTemplate = DataInitializeHelper.GetResourceContent("FormBuilder.RenderScriptTemplate.js", DataSetupResourceType.SiteSetting),
                    SubmitFormBodyTemplate = DataInitializeHelper.GetResourceContent("FormBuilder.SubmitFormBodyTemplate.cshtml", DataSetupResourceType.SiteSetting),
                    SubmitFormSubject = "Form Email Notification"
                }
            };
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Content of render script
        /// </summary>
        [LocalizedDisplayName("SiteSetting_FormBuilderSetting_Field_RenderScriptTemplate")]
        public string RenderScriptTemplate { get; set; }

        /// <summary>
        /// Content of embedded script
        /// </summary>
        [LocalizedDisplayName("SiteSetting_FormBuilderSetting_Field_EmbeddedTemplate")]
        public string EmbeddedTemplate { get; set; }

        /// <summary>
        /// Template for notification
        /// </summary>
        [LocalizedDisplayName("SiteSetting_FormBuilderSetting_Field_SubmitFormBodyTemplate")]
        public string SubmitFormBodyTemplate { get; set; }

        /// <summary>
        /// Content of embedded script
        /// </summary>
        [LocalizedDisplayName("SiteSetting_FormBuilderSetting_Field_SubmitFormSubject")]
        public string SubmitFormSubject { get; set; }

        #endregion
    }
}