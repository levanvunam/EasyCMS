using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.NotificationTemplates;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.NotificationTemplates
{
    public class NotificationTemplateManageModel : IValidatableObject
    {
        #region Constructors

        public NotificationTemplateManageModel()
        {
            Modules = EnumUtilities.GenerateSelectListItems<NotificationEnums.NotificationModule>();
        }

        public NotificationTemplateManageModel(NotificationTemplate notificationTemplate)
            : this()
        {
            Id = notificationTemplate.Id;
            Name = notificationTemplate.Name;
            Subject = notificationTemplate.Subject;
            Body = notificationTemplate.Body;
            IsDefaultTemplate = notificationTemplate.IsDefaultTemplate;
            Module = notificationTemplate.Module;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("NotificationTemplate_Field_Name")]
        public string Name { get; set; }

        [Required]
        [LocalizedDisplayName("NotificationTemplate_Field_Subject")]
        public string Subject { get; set; }

        [Required]
        [LocalizedDisplayName("NotificationTemplate_Field_Body")]
        public string Body { get; set; }

        [Required]
        [LocalizedDisplayName("NotificationTemplate_Field_Module")]
        public NotificationEnums.NotificationModule Module { get; set; }

        public bool IsDefaultTemplate { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> Modules { get; set; }

        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var notificationTemplateService = HostContainer.GetInstance<INotificationTemplateService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (notificationTemplateService.IsNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("NotificationTemplate_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}
