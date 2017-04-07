using Ez.Framework.Configurations;
using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.BackgroundTasks.Models;
using Ez.Framework.Core.Logging;
using Ez.Framework.Utilities;
using Ez.Framework.IoC;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Models.Users.Emails;
using EzCMS.Core.Services.EmailLogs;
using EzCMS.Core.Services.EmailTemplates;
using EzCMS.Core.Services.Users;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System;
using System.Linq;
using System.Threading;
using System.Web;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Core.BackgroundTasks.AccountExpiresNotifications
{
    public class AccountExpiresNotificationTask : IBackgroundTask
    {
        private static int _hasActiveTask;

        /// <summary>
        /// Execute the background task
        /// </summary>
        /// <param name="context"></param>
        public void Run(BackgroundTaskExecuteContext context)
        {
            var logger = HostContainer.GetInstance<ILogger>();
            if (Interlocked.CompareExchange(ref _hasActiveTask, 1, 0) == 0)
            {
                var countUsers = 0;
                try
                {
                    logger.Info(string.Format("[{0}] Start account expires notification task", EzCMSContants.AccountExpiresNotificationTaskName));
                    //Update the background task last running time
                    var backgroundTaskService = HostContainer.GetInstance<IEzBackgroundTaskService>();
                    backgroundTaskService.UpdateLastRunningTimeTask(GetType());

                    var userService = HostContainer.GetInstance<IUserService>();
                    var emailLogService = HostContainer.GetInstance<IEmailLogService>();
                    var emailTemplateService = HostContainer.GetInstance<IEmailTemplateService>();

                    var nearExpirationDateUsers = userService.GetUsersNearExpirationDate().ToList();

                    foreach (var user in nearExpirationDateUsers)
                    {
                        if (user.AccountExpiresDate.HasValue)
                        {
                            var authorizeCode = PasswordUtilities.EncryptString(String.Format("{0}{1}{2}", user.Id,
                                FrameworkConstants.UniqueLinkSeparator, user.Email));
                            var extendExpirationDateLink =
                                UrlUtilities.GenerateUrl(HttpContext.Current.Request.RequestContext, "Account",
                                    "ExtendAccountExpiresDate",
                                    new
                                    {
                                        area = "Admin",
                                        authorizeCode = authorizeCode
                                    }, true);

                            var model = new NotifyAccountExpiredEmailModel
                            {
                                FullName = user.FullName,
                                Username = user.Username,
                                Email = user.Email,
                                ExpirationDate = user.AccountExpiresDate.Value,
                                ExtendExpirationDateLink = extendExpirationDateLink
                            };

                            var emailResponse = emailTemplateService.ParseEmail(EmailEnums.EmailTemplateType.AccountExpiredNotification, model);
                            var emailLog = new EmailLog
                            {
                                To = user.Email,
                                ToName = user.FullName,
                                From = emailResponse.From,
                                FromName = emailResponse.FromName,
                                CC = emailResponse.CC,
                                Bcc = emailResponse.BCC,
                                Subject = emailResponse.Subject,
                                Body = emailResponse.Body,
                                Priority = EmailEnums.EmailPriority.Medium
                            };
                            emailLogService.CreateEmail(emailLog, true);
                            countUsers++;
                        }
                    }

                }
                catch (Exception exception)
                {
                    logger.Error(exception);
                }

                logger.Info(string.Format("[{0}] End account expires notification task. Notify to {1} account(s) near expiration date", EzCMSContants.AccountExpiresNotificationTaskName, countUsers));
                Interlocked.Exchange(ref _hasActiveTask, 0);
            }
        }


    }
}
