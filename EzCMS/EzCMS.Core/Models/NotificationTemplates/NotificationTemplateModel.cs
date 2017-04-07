using Ez.Framework.Models;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.NotificationTemplates
{
    public class NotificationTemplateModel : BaseGridModel
    {
        #region Constructors

        public NotificationTemplateModel()
        {
        }

        public NotificationTemplateModel(NotificationTemplate notificationTemplate)
            : base(notificationTemplate)
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

        public string Name { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsDefaultTemplate { get; set; }

        public NotificationEnums.NotificationModule Module { get; set; }

        public string ModuleName
        {
            get { return Module.GetEnumName(); }
        }

        #endregion
    }
}
