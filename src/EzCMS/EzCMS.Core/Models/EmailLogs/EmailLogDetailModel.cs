using Ez.Framework.Core.Attributes;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Locale.Attributes;
using Ez.Framework.Models;

namespace EzCMS.Core.Models.EmailLogs
{
    public class EmailLogDetailModel : BaseGridModel
    {
        #region Constructors

        public EmailLogDetailModel()
        {
            CcList = new List<string>();
            BccList = new List<string>();
            Logs = new List<EmailSendingLog>();
        }

        public EmailLogDetailModel(EmailLog emailLog)
            : this()
        {
            Id = emailLog.Id;
            Priority = emailLog.Priority;
            From = emailLog.From;
            FromName = emailLog.FromName;
            To = emailLog.To;
            ToName = emailLog.ToName;
            if (!string.IsNullOrEmpty(emailLog.CC))
            {
                CcList = emailLog.CC.Split(';').ToList();
            }
            if (!string.IsNullOrEmpty(emailLog.Bcc))
            {
                BccList = emailLog.Bcc.Split(';').ToList();
            }
            Subject = emailLog.Subject;
            SentTries = emailLog.SentTries;
            SentOn = emailLog.SentOn;
            Body = emailLog.Body;
            EmailAccountId = emailLog.EmailAccountId;
            if (!string.IsNullOrEmpty(emailLog.Message))
            {
                Logs = SerializeUtilities.Deserialize<List<EmailSendingLog>>(emailLog.Message);
            }

            RecordOrder = emailLog.RecordOrder;
            Created = emailLog.Created;
            CreatedBy = emailLog.CreatedBy;
            LastUpdate = emailLog.LastUpdate;
            LastUpdateBy = emailLog.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public EmailEnums.EmailPriority Priority { get; set; }

        [LocalizedDisplayName("EmailLog_Field_From")]
        public string From { get; set; }

        [LocalizedDisplayName("EmailLog_Field_FromName")]
        public string FromName { get; set; }

        [LocalizedDisplayName("EmailLog_Field_To")]
        public string To { get; set; }

        [LocalizedDisplayName("EmailLog_Field_ToName")]
        public string ToName { get; set; }

        [LocalizedDisplayName("EmailLog_Field_CcList")]
        public List<string> CcList { get; set; }

        [LocalizedDisplayName("EmailLog_Field_BccList")]
        public List<string> BccList { get; set; }

        [LocalizedDisplayName("EmailLog_Field_Subject")]
        public string Subject { get; set; }

        [LocalizedDisplayName("EmailLog_Field_Body")]
        public string Body { get; set; }

        [LocalizedDisplayName("EmailLog_Field_SentTries")]
        public int SentTries { get; set; }

        [LocalizedDisplayName("EmailLog_Field_SentOn")]
        public DateTime? SentOn { get; set; }

        public bool IsSent
        {
            get
            {
                return SentOn.HasValue;
            }
        }

        [LocalizedDisplayName("EmailLog_Field_EmailAccountId")]
        public int EmailAccountId { get; set; }

        public List<EmailSendingLog> Logs { get; set; }

        #endregion
    }
}
