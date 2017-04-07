using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.SiteSettings.Models;
using Ez.Framework.IoC;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Core.SiteSettings;

namespace EzCMS.Core.Models.SiteSettings
{
    public class SiteSettingManageModel
    {
        public SiteSettingManageModel()
        {
        }

        public SiteSettingManageModel(SiteSetting setting)
            : this()
        {
            SettingType = setting.SettingType;
            Description = setting.Description;
            SettingName = setting.Name;


            var settingParser = HostContainer.GetInstances<IComplexSetting>().FirstOrDefault(parser => parser.GetSetup().Name.Equals(setting.Name));
            if (settingParser != null)
            {
                Setting = typeof(EzComplexSetting).GetMethod("LoadSetting").MakeGenericMethod(settingParser.GetType()).Invoke(settingParser, null);
            }
            else
            {
                Setting = setting;
            }

        }

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("SiteSetting_Field_SettingName")]
        public string SettingName { get; set; }

        [LocalizedDisplayName("SiteSetting_Field_Description")]
        public string Description { get; set; }

        [LocalizedDisplayName("SiteSetting_Field_Setting")]
        public dynamic Setting { get; set; }

        [LocalizedDisplayName("SiteSetting_Field_Value")]
        public string Value { get; set; }

        [Required]
        [LocalizedDisplayName("SiteSetting_Field_SettingType")]
        public string SettingType { get; set; }

        #endregion

    }
}
