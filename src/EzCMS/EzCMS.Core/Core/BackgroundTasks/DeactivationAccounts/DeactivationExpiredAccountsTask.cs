using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.BackgroundTasks.Models;
using Ez.Framework.Core.Logging;
using Ez.Framework.IoC;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Services.Users;
using System;
using System.Linq;
using System.Threading;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Core.BackgroundTasks.DeactivationAccounts
{
    public class DeactivationExpiredAccountsTask : IBackgroundTask
    {
        private static int _hasActiveTask;

        /// <summary>
        /// Execute the background task
        /// </summary>
        /// <param name="context"></param>
        public void Run(BackgroundTaskExecuteContext context)
        {
            var logger = HostContainer.GetInstance<ILogger>();
            var countUsers = 0;
            if (Interlocked.CompareExchange(ref _hasActiveTask, 1, 0) == 0)
            {
                try
                {
                    logger.Info(string.Format("[{0}] Start deactivation expired accounts task", EzCMSContants.DeactivationExpiredAccountsTaskName));
                    //Update the background task last running time
                    var backgroundTaskService = HostContainer.GetInstance<IEzBackgroundTaskService>();
                    backgroundTaskService.UpdateLastRunningTimeTask(GetType());

                    var userService = HostContainer.GetInstance<IUserService>();
                    var usersNeedDeactive = userService.GetUsersNeedDeactive().ToList();
                    foreach (var user in usersNeedDeactive)
                    {
                        userService.DeactiveExpiredAccount(user);
                        userService.SendDeactivationEmail(user);
                        countUsers++;
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(exception);
                }

                logger.Info(string.Format("[{0}] End deactivation expired accounts task. Deactivate {1} account(s)", EzCMSContants.DeactivationExpiredAccountsTaskName, countUsers));
                Interlocked.Exchange(ref _hasActiveTask, 0);
            }
        }


    }
}
