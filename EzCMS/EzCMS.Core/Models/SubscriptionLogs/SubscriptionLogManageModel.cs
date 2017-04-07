using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Entity.Core.Enums;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Services.SiteSettings;

namespace EzCMS.Core.Models.SubscriptionLogs
{
    public class SubscriptionLogManageModel
    {
        #region Constructors

        public SubscriptionLogManageModel()
        {
            var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
            SubscriptionConfiguration = siteSettingService.LoadSetting<SubscriptionConfigurationSetting>();
        }

        public SubscriptionLogManageModel(SubscriptionEnums.SubscriptionModule module, string parameters) : this()
        {
            Module = module;
            Parameters = parameters;
        }

        public SubscriptionLogManageModel(SubscriptionEnums.SubscriptionModule module, dynamic parameters) : this()
        {
            Module = module;
            Parameters = SerializeUtilities.Serialize(parameters);
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SubscriptionLog_Field_NotifySubscribers")]
        public bool NotifySubscribers { get; set; }

        [LocalizedDisplayName("SubscriptionLog_Field_ChangeLog")]
        public string ChangeLog { get; set; }

        [LocalizedDisplayName("SubscriptionLog_Field_Parameters")]
        public string Parameters { get; set; }

        [LocalizedDisplayName("SubscriptionLog_Field_Module")]
        public SubscriptionEnums.SubscriptionModule Module { get; set; }

        public SubscriptionConfigurationSetting SubscriptionConfiguration { get; set; }

        #endregion
    }
}
