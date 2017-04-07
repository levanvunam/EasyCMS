using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Notifications.ModuleParameters;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Services.Localizes;
using EzCMS.Entity.Core.Enums;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Services.SiteSettings;

namespace EzCMS.Core.Models.Notifications.Setup
{
    public class NotificationInitializeModel : NotificationSetupModel, IValidatableObject
    {
        #region Constructors

        public NotificationInitializeModel()
        {
            var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();

            NotificationConfiguration = siteSettingService.LoadSetting<NotificationConfigurationSetting>();
        }

        public NotificationInitializeModel(NotificationEnums.NotificationModule module, string parameters)
            : this()
        {
            Module = module;
            Parameters = parameters;
        }

        public NotificationInitializeModel(NotificationEnums.NotificationModule module, dynamic parameters)
            : this()
        {
            Module = module;
            Parameters = SerializeUtilities.Serialize(parameters);
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("Notification_Field_NotifyContacts")]
        public bool NotifyContacts { get; set; }

        [Required]
        [LocalizedDisplayName("Notification_Field_Parameters")]
        public string Parameters { get; set; }

        public NotificationConfigurationSetting NotificationConfiguration { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            switch (Module)
            {
                case NotificationEnums.NotificationModule.Page:

                    if (SerializeUtilities.Deserialize<NotificationPageParameterModel>(Parameters).Id == 0)
                    {
                        yield return new ValidationResult(localizedResourceService.T("Notification_Message_InvalidParameters"), new[] { "Parameters" });
                    }
                    break;
            }
        }
    }
}
