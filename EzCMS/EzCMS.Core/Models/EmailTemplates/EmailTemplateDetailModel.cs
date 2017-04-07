using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.EmailTemplates
{
    public class EmailTemplateDetailModel
    {
        public EmailTemplateDetailModel()
        {
        }

        public EmailTemplateDetailModel(EmailTemplate emailTemplate)
            : this()
        {
            Id = emailTemplate.Id;

            EmailTemplate = new EmailTemplateManageModel(emailTemplate);

            Created = emailTemplate.Created;
            CreatedBy = emailTemplate.CreatedBy;
            LastUpdate = emailTemplate.LastUpdate;
            LastUpdateBy = emailTemplate.LastUpdateBy;

        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("EmailTemplate_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("EmailTemplate_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("EmailTemplate_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("EmailTemplate_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public EmailTemplateManageModel EmailTemplate { get; set; }

        #endregion
    }
}
