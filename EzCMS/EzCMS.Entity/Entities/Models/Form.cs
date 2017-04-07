using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Form : BaseModel
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public string JsonContent { get; set; }

        public bool Active { get; set; }

        public int? StyleId { get; set; }

        [ForeignKey("StyleId")]
        public virtual Style Style { get; set; }

        #region Configuration

        public string FromName { get; set; }

        public string FromEmail { get; set; }

        public string ThankyouMessage { get; set; }

        public bool AllowAjaxSubmit { get; set; }

        #region Notify Owner

        public bool SendSubmitFormEmail { get; set; }

        public string EmailTo { get; set; }

        #endregion

        #region Notification

        public bool SendNotificationEmail { get; set; }

        public string NotificationSubject { get; set; }

        public string NotificationBody { get; set; }

        public string NotificationEmailTo { get; set; }

        #endregion

        #region Auto Response

        public bool SendAutoResponse { get; set; }

        public string AutoResponseSubject { get; set; }

        public string AutoResponseBody { get; set; }

        #endregion

        #endregion
    }
}