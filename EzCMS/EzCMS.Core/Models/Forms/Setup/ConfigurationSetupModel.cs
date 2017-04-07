using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using EzCMS.Core.Services.Styles;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Forms.Setup
{
    public class ConfigurationSetupModel : FormSetupModel
    {
        public ConfigurationSetupModel()
        {
            Step = ForNavigationms.FormSetupStep.SetupConfiguration;

            var styleService = HostContainer.GetInstance<IStyleService>();
            Styles = styleService.GetStyles();
        }

        public ConfigurationSetupModel(Form form)
            : this()
        {
            var styleService = HostContainer.GetInstance<IStyleService>();
            Styles = styleService.GetStyles(form.StyleId);

            Id = form.Id;

            Name = form.Name;
            StyleId = form.StyleId;
            FromName = form.FromName;
            FromEmail = form.FromEmail;
            ThankyouMessage = form.ThankyouMessage;
            AllowAjaxSubmit = form.AllowAjaxSubmit;

            SendSubmitFormEmail = form.SendSubmitFormEmail;
            EmailTo = form.EmailTo;

            SendNotificationEmail = form.SendNotificationEmail;
            NotificationSubject = form.NotificationSubject;
            NotificationBody = form.NotificationBody;
            NotificationEmailTo = form.NotificationEmailTo;

            SendAutoResponse = form.SendAutoResponse;
            AutoResponseSubject = form.AutoResponseSubject;
            AutoResponseBody = form.AutoResponseBody;
        }

        #region Public Properties

        [Required]
        [LocalizedDisplayName("Form_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("Form_Field_StyleId")]
        public int? StyleId { get; set; }

        public IEnumerable<SelectListItem> Styles { get; set; }

        [Required]
        [LocalizedDisplayName("Form_Field_ThankyouMessage")]
        public string ThankyouMessage { get; set; }

        [Required]
        [LocalizedDisplayName("Form_Field_FromName")]
        public string FromName { get; set; }

        [Required]
        [EmailValidation]
        [LocalizedDisplayName("Form_Field_FromEmail")]
        public string FromEmail { get; set; }

        [LocalizedDisplayName("Form_Field_AllowAjaxSubmit")]
        public bool AllowAjaxSubmit { get; set; }

        #region Notify Owner

        [LocalizedDisplayName("Form_Field_SendSubmitFormEmail")]
        public bool SendSubmitFormEmail { get; set; }

        [EmailValidation]
        [RequiredIf("SendSubmitFormEmail", true)]
        [LocalizedDisplayName("Form_Field_EmailTo")]
        public string EmailTo { get; set; }

        #endregion

        #region Notification

        [LocalizedDisplayName("Form_Field_SendNotificationEmail")]
        public bool SendNotificationEmail { get; set; }

        [RequiredIf("SendNotificationEmail", true)]
        [LocalizedDisplayName("Form_Field_NotificationSubject")]
        public string NotificationSubject { get; set; }

        [RequiredIf("SendNotificationEmail", true)]
        [LocalizedDisplayName("Form_Field_NotificationBody")]
        public string NotificationBody { get; set; }

        [EmailValidation]
        [RequiredIf("SendNotificationEmail", true)]
        [LocalizedDisplayName("Form_Field_NotificationEmailTo")]
        public string NotificationEmailTo { get; set; }

        #endregion

        #region Auto Response

        [LocalizedDisplayName("Form_Field_SendAutoResponse")]
        public bool SendAutoResponse { get; set; }

        [RequiredIf("SendAutoResponse", true)]
        [LocalizedDisplayName("Form_Field_AutoResponseSubject")]
        public string AutoResponseSubject { get; set; }

        [RequiredIf("SendAutoResponse", true)]
        [LocalizedDisplayName("Form_Field_AutoResponseBody")]
        public string AutoResponseBody { get; set; }

        #endregion

        #endregion
    }
}
