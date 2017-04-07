using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.EmailTemplates
{
    public class EmailTemplateSendTestModel
    {
        #region Constructors

        public EmailTemplateSendTestModel()
        {
        }

        public EmailTemplateSendTestModel(EmailTemplate emailTemplate) 
            : this()
        {
            Id = emailTemplate.Id;
            Name = emailTemplate.Name;
            Subject = emailTemplate.Subject;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [EmailValidation]
        [LocalizedDisplayName("EmailTemplate_Field_ToTestEmailTemplate")]
        public string To { get; set; }

        [LocalizedDisplayName("EmailTemplate_Field_CCTestEmailTemplate")]
        public string CC{ get; set; }

        [LocalizedDisplayName("EmailTemplate_Field_BCCTestEmailTemplate")]
        public string BCC { get; set; }

        [Required]
        [LocalizedDisplayName("EmailTemplate_Field_SubjectTestEmailTemplate")]
        public string Subject { get; set; }

        public string Name { get; set; }

        #endregion
    }
}
