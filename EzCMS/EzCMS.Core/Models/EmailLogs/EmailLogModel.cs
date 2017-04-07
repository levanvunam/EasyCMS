using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Models;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System;

namespace EzCMS.Core.Models.EmailLogs
{
    public class EmailLogModel : BaseGridModel
    {
        public EmailLogModel()
        {

        }

        public EmailLogModel(EmailLog emailLog) : this()
        {
            Priority = emailLog.Priority;
            From = emailLog.From;
            FromName = emailLog.FromName;
            To = emailLog.To;
            ToName = emailLog.ToName;
            CC = emailLog.CC;
            Bcc = emailLog.Bcc;
            Subject = emailLog.Subject;
            SendLater = emailLog.SendLater;
            SentTries = emailLog.SentTries;
            SentOn = emailLog.SentOn;
            EmailAccountId = emailLog.EmailAccountId;

            RecordOrder = emailLog.RecordOrder;
            Created = emailLog.Created;
            CreatedBy = emailLog.CreatedBy;
            LastUpdate = emailLog.LastUpdate;
            LastUpdateBy = emailLog.LastUpdateBy;
        }

        #region Public Properties

        public EmailEnums.EmailPriority Priority { get; set; }

        public string From { get; set; }

        public string FromName { get; set; }

        public string To { get; set; }

        public string ToName { get; set; }

        public string CC { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public int SentTries { get; set; }

        public DateTime? SendLater { get; set; }

        public DateTime? SentOn { get; set; }

        public bool IsSent { get; set; }

        public int EmailAccountId { get; set; }

        [DefaultOrder(Order = GridSort.Desc)]
        public new DateTime Created { get; set; }

        #endregion

    }
}
