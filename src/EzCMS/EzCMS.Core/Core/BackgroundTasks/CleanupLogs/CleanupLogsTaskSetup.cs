using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.Entity.Enums;
using Ez.Framework.Core.Entity.Extensions;
using Ez.Framework.Core.Entity.Intialize;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Entity.Entities;
using System;
using System.Data.Entity;

namespace EzCMS.Core.Core.BackgroundTasks.CleanupLogs
{
    public class CleanupLogsTaskSetup : IBackgroundTaskSetup
    {
        /// <summary>
        /// Get background task name
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return EzCMSContants.CleanupLogsTaskName;
        }

        /// <summary>
        /// Get job
        /// </summary>
        /// <returns></returns>
        public IBackgroundTask GetJob()
        {
            return new CleanupLogsTask();
        }


        #region Initialize Task Configuration

        public DataInitializerPriority Priority()
        {
            return DataInitializerPriority.Medium;
        }

        public void Initialize(DbContext ezDbContext)
        {
            EzCMSEntities dbContext = ezDbContext as EzCMSEntities;
            if (dbContext != null)
            {
                var backgroundTask = new BackgroundTask
                {
                    Name = GetName(),
                    Description = GetName(),
                    Status = BackgroundTaskEnums.TaskStatus.Active,
                    ScheduleType = BackgroundTaskEnums.ScheduleType.Daily,
                    StartTime = new TimeSpan(1, 0, 0)
                };

                dbContext.BackgroundTasks.AddIfNotExist(t => t.Name, backgroundTask);
            }
        }

        #endregion
    }
}
