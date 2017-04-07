using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.BackgroundTasks.Models;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Logging;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Services.PageLogs;
using EzCMS.Core.Services.PageTemplateLogs;
using EzCMS.Core.Services.ProtectedDocumentLogs;
using EzCMS.Core.Services.ScriptLogs;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Core.Services.SocialMediaLogs;
using EzCMS.Core.Services.StyleLogs;
using EzCMS.Core.Services.SubscriptionLogs;
using EzCMS.Core.Services.WidgetTemplateLogs;
using System;
using System.Threading;

namespace EzCMS.Core.Core.BackgroundTasks.CleanupLogs
{
    public class CleanupLogsTask : IBackgroundTask
    {
        private static int _hasActiveTask;

        public void Run(BackgroundTaskExecuteContext context)
        {
            var logger = HostContainer.GetInstance<ILogger>();
            if (Interlocked.CompareExchange(ref _hasActiveTask, 1, 0) == 0)
            {
                logger.Info(string.Format("[{0}] Start cleanup logs task", EzCMSContants.CleanupLogsTaskName));
                try
                {
                    var backgroundTaskService = HostContainer.GetInstance<IEzBackgroundTaskService>();
                    backgroundTaskService.UpdateLastRunningTimeTask(GetType());

                    #region Defined services

                    var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
                    var pageLogService = HostContainer.GetInstance<IPageLogService>();
                    var pageTemplateLogService = HostContainer.GetInstance<IPageTemplateLogService>();
                    var protectedDocumentLogService = HostContainer.GetInstance<IProtectedDocumentLogService>();
                    var scriptLogService = HostContainer.GetInstance<IScriptLogService>();
                    var socialMediaLogService = HostContainer.GetInstance<ISocialMediaLogService>();
                    var styleLogService = HostContainer.GetInstance<IStyleLogService>();
                    var subscriptionLogService = HostContainer.GetInstance<ISubscriptionLogService>();
                    var templateLogService = HostContainer.GetInstance<IWidgetTemplateLogService>();

                    #endregion

                    var settings = siteSettingService.LoadSetting<CleanupLogsSetting>();

                    if (settings != null)
                    {
                        // Delete page logs
                        pageLogService.DeletePageLogs(DateTime.Now.AddDays(-settings.PageMaxBackupLogDays));

                        // Delete page template logs
                        pageTemplateLogService.DeletePageTemplateLogs(DateTime.Now.AddDays(-settings.PageTemplateMaxBackupLogDays));

                        // Delete protected document logs
                        protectedDocumentLogService.DeleteProtectedDocumentLogs(DateTime.Now.AddDays(-settings.ProtectedDocumentMaxBackupLogDays));

                        // Delete script logs
                        scriptLogService.DeleteScriptLogs(DateTime.Now.AddDays(-settings.ScriptMaxBackupLogDays));

                        // Delete social media logs
                        socialMediaLogService.DeleteSocialMediaLogs(DateTime.Now.AddDays(-settings.SocialMediaMaxBackupLogDays));

                        // Delete style logs
                        styleLogService.DeleteStyleLogs(DateTime.Now.AddDays(-settings.StyleMaxBackupLogDays));

                        // Delete subscription logs
                        subscriptionLogService.DeleteSubscriptionLogs(DateTime.Now.AddDays(-settings.SubscriptionMaxBackupLogDays));

                        // Delete template logs
                        templateLogService.DeleteTemplateLogs(DateTime.Now.AddDays(-settings.TemplateMaxBackupLogDays));
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(exception);
                }

                logger.Info(string.Format("[{0}] End cleanup logs task", EzCMSContants.CleanupLogsTaskName));
                Interlocked.Exchange(ref _hasActiveTask, 0);
            }
        }
    }
}
