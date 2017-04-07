using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.BackgroundTasks.Models;
using Ez.Framework.Core.Logging;
using Ez.Framework.IoC;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Models.Subscriptions.Emails;
using EzCMS.Core.Models.Subscriptions.Subscribers;
using EzCMS.Core.Services.EmailLogs;
using EzCMS.Core.Services.EmailTemplates;
using EzCMS.Core.Services.SubscriptionLogs;
using EzCMS.Core.Services.Subscriptions;
using EzCMS.Core.Services.SubscriptionTemplates;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System;
using System.Linq;
using System.Threading;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Core.BackgroundTasks.Subscriptions.DirectlyTask
{
    public class SubscriptionDirectlyTask : IBackgroundTask
    {
        private static int _hasActiveTask;

        /// <summary>
        /// Execute Instantly Tasks
        /// </summary>
        /// <param name="context"></param>
        public void Run(BackgroundTaskExecuteContext context)
        {
            var logger = HostContainer.GetInstance<ILogger>();
            if (Interlocked.CompareExchange(ref _hasActiveTask, 1, 0) == 0)
            {
                try
                {
                    logger.Info(string.Format("[{0}] Start subscription directly task", EzCMSContants.SubscriptionDirectlyTaskName));

                    // Update the background task last running time
                    var backgroundTaskService = HostContainer.GetInstance<IEzBackgroundTaskService>();
                    backgroundTaskService.UpdateLastRunningTimeTask(GetType());

                    var subscriptionService = HostContainer.GetInstance<ISubscriptionService>();
                    var subscriptionLogService = HostContainer.GetInstance<ISubscriptionLogService>();
                    var subscriptionTemplateService = HostContainer.GetInstance<ISubscriptionTemplateService>();
                    var emailTemplateService = HostContainer.GetInstance<IEmailTemplateService>();
                    var emailLogService = HostContainer.GetInstance<IEmailLogService>();

                    var subscriptionLogs = subscriptionLogService.GetLogs(SubscriptionEnums.SubscriptionType.Instantly);

                    if (subscriptionLogs.Any())
                    {
                        var templates = subscriptionTemplateService.GetAll().ToList();

                        // Get a list of all emails in subscription table
                        // Each email will have a list of subscribed items
                        var subscribers =
                            subscriptionService.Fetch(s => s.Contact.SubscriptionType == SubscriptionEnums.SubscriptionType.Instantly)
                                .GroupBy(s => s.Email)
                                .Select(s => new SubscriberModel
                                {
                                    Email = s.Key,
                                    SubscribeModules = s.Select(m => new SubscribeModule
                                    {
                                        SubscriptionId = m.Id,
                                        Module = m.Module,
                                        Parameters = m.Parameters
                                    }).ToList()
                                });

                        foreach (var subscriber in subscribers)
                        {
                            var emailModel = new SubscribeNoticationEmailModel
                            {
                                Email = subscriber.Email,
                                Content = string.Empty
                            };

                            var isAnythingChanged = false;

                            // Loop through all available templates to get the final html to send
                            foreach (var template in templates)
                            {
                                var currentModule = template.Module;
                                var moduleLogs = subscriptionLogs.Where(l => l.Module == currentModule);

                                // Get subscription item of module
                                var subscriberInfo = subscriber.SubscribeModules.FirstOrDefault(m => m.Module == template.Module);

                                // Add the module html to master content
                                emailModel.Content = subscriptionService.GetSubscriptionEmailContent(new[] { subscriberInfo }, moduleLogs, template, ref isAnythingChanged);
                            }

                            // If there is nothing changed, we don't send email
                            if (!isAnythingChanged) continue;

                            // Parse content into master template
                            var emailResponse = emailTemplateService.ParseEmail(EmailEnums.EmailTemplateType.SubscribeNotification, emailModel);

                            // Create email in EmailLog, ready to be sent
                            emailLogService.CreateEmail(new EmailLog
                            {
                                From = emailResponse.From,
                                FromName = emailResponse.FromName,
                                CC = emailResponse.CC,
                                Bcc = emailResponse.BCC,
                                To = emailModel.Email,
                                Subject = emailResponse.Subject,
                                Body = emailResponse.Body
                            });
                        }

                        // Disable directly log
                        foreach (var subscriptionLog in subscriptionLogs.ToList())
                        {
                            subscriptionLogService.DisabledSubscriptionDirectly(subscriptionLog);
                        }
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(exception);
                }
                logger.Info(string.Format("[{0}] End subscription directly task", EzCMSContants.SubscriptionDirectlyTaskName));
                Interlocked.Exchange(ref _hasActiveTask, 0);
            }
        }
    }
}
