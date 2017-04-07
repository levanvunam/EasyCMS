using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.NotificationTemplates
{
    public class NotificationTemplateDetailModel
    {
        public NotificationTemplateDetailModel()
        {

        }

        public NotificationTemplateDetailModel(NotificationTemplate notificationTemplate)
            : this()
        {
            Id = notificationTemplate.Id;

            NotificationTemplate = new NotificationTemplateManageModel(notificationTemplate);

            Created = notificationTemplate.Created;
            CreatedBy = notificationTemplate.CreatedBy;
            LastUpdate = notificationTemplate.LastUpdate;
            LastUpdateBy = notificationTemplate.LastUpdateBy;
        }

        #region Public Properties

        public int Id { get; set; }

        [LocalizedDisplayName("NotificationTemplate_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("NotificationTemplate_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("NotificationTemplate_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("NotificationTemplate_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public NotificationTemplateManageModel NotificationTemplate { get; set; }

        #endregion
    }
}
