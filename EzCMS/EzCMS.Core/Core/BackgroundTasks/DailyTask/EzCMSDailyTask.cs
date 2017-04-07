using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.BackgroundTasks.Models;
using Ez.Framework.Core.Logging;
using Ez.Framework.IoC;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Services.SocialMediaTokens;
using System;
using System.Threading;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Core.BackgroundTasks.DailyTask
{
    public class EzCMSDailyTask : IBackgroundTask
    {
        private static int _hasActiveTask;

        public void Run(BackgroundTaskExecuteContext context)
        {
            var logger = HostContainer.GetInstance<ILogger>();
            if (Interlocked.CompareExchange(ref _hasActiveTask, 1, 0) == 0)
            {
                try
                {
                    logger.Info(string.Format("[{0}] Start EzCMS daily task", EzCMSContants.EzCMSDailyTaskName));
                    var backgroundTaskService = HostContainer.GetInstance<IEzBackgroundTaskService>();
                    backgroundTaskService.UpdateLastRunningTimeTask(GetType());

                    #region Checking Social Media Status

                    var socialMediaTokenService = HostContainer.GetInstance<ISocialMediaTokenService>();

                    socialMediaTokenService.CheckAndUpdateTokenStatus();

                    #endregion
                }
                catch (Exception exception)
                {
                    logger.Error(exception);
                }

                logger.Info(string.Format("[{0}] End EzCMS daily task", EzCMSContants.EzCMSDailyTaskName));
                Interlocked.Exchange(ref _hasActiveTask, 0);
            }
        }
    }
}
