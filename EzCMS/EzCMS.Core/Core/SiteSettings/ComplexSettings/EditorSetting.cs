using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Framework.SiteSettings;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class EditorSetting : ComplexSetting
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
                Name = SettingNames.EditorSetting,
                SettingType = "system",
                DefaultValue = new EditorSetting
                {
                    ScriptContent = DataInitializeHelper.GetResourceContent("EditorSetting.editor.js", DataSetupResourceType.SiteSetting),
                    StyleContent = DataInitializeHelper.GetResourceContent("EditorSetting.editor.css", DataSetupResourceType.SiteSetting)
                }
            };
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Content of script
        /// </summary>
        [LocalizedDisplayName("SiteSetting_EditorSetting_Field_ScriptContent")]
        public string ScriptContent { get; set; }

        /// <summary>
        /// Content of style
        /// </summary>
        [LocalizedDisplayName("SiteSetting_EditorSetting_Field_StyleContent")]
        public string StyleContent { get; set; }

        #endregion
    }
}