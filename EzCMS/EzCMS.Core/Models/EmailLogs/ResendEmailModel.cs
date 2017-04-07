using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.EmailAccounts;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.EmailLogs
{
    public class ResendEmailModel
    {
        public ResendEmailModel()
        {
            var emailAccountService = HostContainer.GetInstance<IEmailAccountService>();
            EmailAccounts = emailAccountService.GetEmailAccounts();
        }

        public ResendEmailModel(EmailLog emailLog) : this()
        {
            EmailLogId = emailLog.Id;
            Subject = emailLog.Subject;
            To = emailLog.To;
            ToName = emailLog.ToName;

        }

        #region Public Properties

        public int EmailLogId { get; set; }
		
        [LocalizedDisplayName("EmailLog_Field_Subject")]
        public string Subject { get; set; }

        public string To { get; set; }

        public string ToName { get; set; }


        [LocalizedDisplayName("EmailLog_Field_EmailAccountId")]
        public int EmailAccountId { get; set; }

        public IEnumerable<SelectListItem> EmailAccounts { get; set; } 

        #endregion
    }
}
