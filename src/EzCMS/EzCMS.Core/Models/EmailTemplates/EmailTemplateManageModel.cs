using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Services.Localizes;
using EzCMS.Entity.Entities.Models;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.EmailTemplates
{
    public class EmailTemplateManageModel : IValidatableObject
    {
        #region Constructors
        public EmailTemplateManageModel()
        {
        }

        public EmailTemplateManageModel(EmailTemplate emailTemplate)
            : this()
        {
            Id = emailTemplate.Id;

            From = emailTemplate.From;
            FromName = emailTemplate.FromName;
            CC = emailTemplate.CC;
            BCC = emailTemplate.BCC;

            Subject = emailTemplate.Subject;
            Body = emailTemplate.Body;
            DataType = emailTemplate.DataType;

            TypeName = emailTemplate.Type.GetEnumName();
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("EmailTemplate_Field_Subject")]
        public string Subject { get; set; }

        [EmailValidation]
        [LocalizedDisplayName("EmailTemplate_Field_From")]
        public string From { get; set; }

        [LocalizedDisplayName("EmailTemplate_Field_FromName")]
        public string FromName { get; set; }

        [LocalizedDisplayName("EmailTemplate_Field_CC")]
        public string CC { get; set; }

        [LocalizedDisplayName("EmailTemplate_Field_BCC")]
        public string BCC { get; set; }

        [Required]
        [LocalizedDisplayName("EmailTemplate_Field_Body")]
        public string Body { get; set; }

        [LocalizedDisplayName("EmailTemplate_Field_TypeName")]
        public string TypeName { get; set; }

        [LocalizedDisplayName("EmailTemplate_Field_DataType")]
        public string DataType { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            var type = Type.GetType(DataType);
            if (type != null)
            {
                var razorValidMessage = string.Empty;
                try
                {
                    var cacheName = Subject.GetTemplateCacheName(Body);
                    EzRazorEngineHelper.TryCompileAndAddTemplate(Body, cacheName, type);
                }
                catch (TemplateParsingException exception)
                {
                    razorValidMessage = exception.Message;
                }
                catch (TemplateCompilationException exception)
                {
                    razorValidMessage = string.Join("\n", exception.CompilerErrors.Select(e => e.ErrorText));
                }
                catch (Exception exception)
                {
                    razorValidMessage = exception.Message;
                }
                if (!string.IsNullOrEmpty(razorValidMessage))
                    yield return new ValidationResult(string.Format(localizedResourceService.T("EmailTemplate_Message_TemplateCompileFailure"), razorValidMessage), new[] { "Body" });
            }
        }
    }
}
