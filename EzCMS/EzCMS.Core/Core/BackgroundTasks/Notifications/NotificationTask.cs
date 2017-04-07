using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.BackgroundTasks.Models;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Logging;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Models.Notifications;
using EzCMS.Core.Models.Notifications.ModuleParameters;
using EzCMS.Core.Models.Notifications.NotificationTemplates;
using EzCMS.Core.Models.Pages;
using EzCMS.Core.Services.ContactGroups;
using EzCMS.Core.Services.EmailLogs;
using EzCMS.Core.Services.Notifications;
using EzCMS.Core.Services.NotificationTemplates;
using EzCMS.Core.Services.Pages;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System;
using System.Linq;
using System.Threading;

namespace EzCMS.Core.Core.BackgroundTasks.Notifications
{
    public class NotificationTask : IBackgroundTask
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
                var countNotifications = 0;
                var countContacts = 0;
                try
                {
                    // Update the background task last running time
                    var backgroundTaskService = HostContainer.GetInstance<IEzBackgroundTaskService>();
                    backgroundTaskService.UpdateLastRunningTimeTask(GetType());

                    var notificationService = HostContainer.GetInstance<INotificationService>();
                    var contactGroupService = HostContainer.GetInstance<IContactGroupService>();
                    var emailLogService = HostContainer.GetInstance<IEmailLogService>();

                    // Get in queue notifications
                    var notifications = notificationService.GetNotificationQueues().ToList();
                    countNotifications = notifications.Count;

                    foreach (var notification in notifications)
                    {
                        var notificationTemplate = new NotificationTemplate
                        {
                            Subject = notification.NotificationSubject,
                            Body = notification.NotificationBody
                        };

                        // Get contacts from contact group in notification
                        var contacts = contactGroupService.GetContacts(notification.ContactQueries).ToList();
                        countContacts = contacts.Count;

                        // Save found contacts
                        var notifiedContactModels = contacts.Select(contact => new NotifiedContactModel(contact)).ToList();
                        notification.NotifiedContacts = SerializeUtilities.Serialize(notifiedContactModels);
                        notificationService.UpdateNotification(notification);

                        // Loop through notification email model to get the final html to send
                        foreach (var contact in contacts)
                        {
                            var emailBody = string.Empty;
                            switch (notification.Module)
                            {
                                case NotificationEnums.NotificationModule.Page:
                                    emailBody = GetPageNotificationEmailContent(notification, notificationTemplate, contact);
                                    break;
                            }

                            // Create email in email queue, ready to be sent
                            var response = emailLogService.CreateEmail(new EmailLog
                            {
                                To = contact.Email,
                                ToName = contact.FullName,
                                Subject = notificationTemplate.Subject,
                                Body = emailBody,
                                SendLater = notification.SendTime
                            });

                            // Deactive notification if email queue created successfully
                            if (response.Success)
                            {
                                notificationService.DeactiveNotification(notification);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(string.Format("[{0}]", EzCMSContants.NotificationTaskName), exception);
                }

                if (countContacts > 0)
                {
                    logger.Info(string.Format("[{0}] Send {1} notification(s) to {2} contact(s)", EzCMSContants.NotificationTaskName, countNotifications, countContacts));
                }

                Interlocked.Exchange(ref _hasActiveTask, 0);
            }
        }

        /// <summary>
        /// Build html for page module
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="notificationTemplate"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        private string GetPageNotificationEmailContent(Notification notification, NotificationTemplate notificationTemplate, Contact contact)
        {
            var pageService = HostContainer.GetInstance<IPageService>();
            var notificationTemplateService = HostContainer.GetInstance<INotificationTemplateService>();

            // Get page parameters in notification model
            var pageParameters = SerializeUtilities.Deserialize<NotificationPageParameterModel>(notification.Parameters);

            // Map that page parameters to page model
            var page = pageService.GetById(pageParameters.Id);

            // Create a page notification email model from page model
            var model = new NotificationPageEmailModel
            {
                NotificationId = notification.Id,
                Contact = new ContactModel(contact),
                Page = page != null ? new PageModel(page) : new PageModel()
            };

            return notificationTemplateService.ParseNotification(notificationTemplate, model).Body;
        }
    }
}
