using Ez.Framework.Models;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.EmailTemplates
{
    public class EmailTemplateModel : BaseGridModel
    {
        #region Constructors
        public EmailTemplateModel()
        {

        }

        public EmailTemplateModel(EmailTemplate emailTemplate)
            : this()
        {
            Id = emailTemplate.Id;
            Subject = emailTemplate.Subject;
            Type = emailTemplate.Type;
            From = emailTemplate.From;
            FromName = emailTemplate.FromName;
            CC = emailTemplate.CC;
            BCC = emailTemplate.BCC;

            RecordOrder = emailTemplate.RecordOrder;
            Created = emailTemplate.Created;
            CreatedBy = emailTemplate.CreatedBy;
            LastUpdate = emailTemplate.LastUpdate;
            LastUpdateBy = emailTemplate.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public string From { get; set; }

        public string FromName { get; set; }

        public string CC { get; set; }

        public string BCC { get; set; }

        public string Subject { get; set; }

        public EmailEnums.EmailTemplateType Type { get; set; }

        public string TypeName
        {
            get { return Type.GetEnumName(); }
        }

        #endregion
    }
}
