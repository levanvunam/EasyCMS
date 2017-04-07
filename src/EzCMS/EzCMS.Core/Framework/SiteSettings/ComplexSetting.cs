using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.Entity.Extensions;
using Ez.Framework.Core.SiteSettings.Models;
using Ez.Framework.Utilities;
using EzCMS.Entity.Entities;
using System.Data.Entity;

namespace EzCMS.Core.Framework.SiteSettings
{
    public abstract class ComplexSetting : EzComplexSetting
    {
        public override void Initialize(DbContext ezDbContext)
        {
            EzCMSEntities dbContext = ezDbContext as EzCMSEntities;
            if (dbContext != null)
            {
                var setting = GetSetup().DefaultValue;
                var settingString = SerializeUtilities.Serialize(setting);
                var insertSetting = new SiteSetting
                {
                    Name = GetSetup().Name,
                    Description = GetSetup().Description,
                    Value = settingString,
                    SettingType = GetSetup().SettingType
                };
                dbContext.SiteSettings.AddIfNotExist(i => i.Name, insertSetting);
            }
        }
    }
}
