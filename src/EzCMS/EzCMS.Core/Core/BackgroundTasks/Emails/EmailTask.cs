using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.BackgroundTasks.Models;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Logging;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Mails;
using Ez.Framework.Utilities.Mails.Models;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Models.EmailLogs;
using EzCMS.Core.Services.EmailAccounts;
using EzCMS.Core.Services.EmailLogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EzCMS.Core.Core.BackgroundTasks.Emails
{
    public class EmailTask : IBackgroundTask
    {
        private static int _hasActiveTask;
        public void Run(BackgroundTaskExecuteContext context)
        {
            var logger = HostContainer.GetInstance<ILogger>();
            if (Interlocked.CompareExchange(ref _hasActiveTask, 1, 0) == 0)
            {
                var countEmails = 0;
                try
                {
                    // Update the background task last running time
                    var backgroundTaskService = HostContainer.GetInstance<IEzBackgroundTaskService>();
                    backgroundTaskService.UpdateLastRunningTimeTask(GetType());

                    var emailLogService = HostContainer.GetInstance<IEmailLogService>();
                    var emailAccountService = HostContainer.GetInstance<IEmailAccountService>();
                    var emailAccounts = emailAccountService.GetAll().ToList();
                    var emailLogs = emailLogService.GetEmailSendingQueues();
                    countEmails = emailLogs.Count();
                    foreach (var emailLog in emailLogs)
                    {
                        var emailAccount = emailAccounts.FirstOrDefault(e => e.Id == emailLog.EmailAccountId);
                        if (emailAccount != null)
                        {
                            var emailSetting = new EmailSetting
                            {
                                Host = emailAccount.Host,
                                Port = emailAccount.Port,
                                User = emailAccount.Username,
                                Password = emailAccount.Password,
                                EnableSsl = emailAccount.EnableSsl,
                                Timeout = emailAccount.TimeOut
                            };
                            var mailUtilities = new MailUtilities(emailSetting);
                            var logs = new List<EmailSendingLog>();
                            try
                            {
                                if (!string.IsNullOrEmpty(emailLog.Message))
                                {
                                    logs = SerializeUtilities.Deserialize<List<EmailSendingLog>>(emailLog.Message);
                                }

                                mailUtilities.SendEmail(emailLog.From ?? emailAccount.Email, emailLog.FromName ?? emailAccount.DisplayName, emailLog.To, emailLog.CC, emailLog.Bcc,
                                    true, emailLog.Subject, emailLog.Body);
                                emailLog.SentOn = DateTime.UtcNow;

                                logs.Add(new EmailSendingLog
                                {
                                    Time = DateTime.Now,
                                    Message = string.Format("Mail sent at {0} UTC Time", DateTime.UtcNow)
                                });
                            }
                            catch (Exception exception)
                            {
                                logs.Add(new EmailSendingLog
                                {
                                    Time = DateTime.Now,
                                    Message = string.Format("Error: {0}", exception.Message)
                                });
                            }

                            emailLog.SentTries++;
                            emailLog.Message = SerializeUtilities.Serialize(logs);
                            emailLogService.UpdateMail(emailLog);
                        }
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(string.Format("[{0}]", EzCMSContants.EmailTaskName), exception);
                }
                if (countEmails > 0)
                {
                    logger.Info(string.Format("[{0}] Send {1} email(s) in queues", EzCMSContants.EmailTaskName, countEmails));
                }
                Interlocked.Exchange(ref _hasActiveTask, 0);
            }
        }
    }
}