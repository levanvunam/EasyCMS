using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.Entity.Enums;
using Ez.Framework.Core.Entity.Extensions;
using Ez.Framework.Core.Entity.Intialize;
using EzCMS.Core.Framework.Configuration;
using System.Data.Entity;

namespace EzCMS.Core.Core.BackgroundTasks.Notifications
{
    public class NotificationTaskSetup : IBackgroundTaskSetup
    {
        /// <summary>
        /// Get background task name
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return EzCMSContants.NotificationTaskName;
        }

        /// <summary>
        /// Get job
        /// </summary>
        /// <returns></returns>
        public IBackgroundTask GetJob()
        {
            return new NotificationTask();
        }

        #region Initialize Task Configuration

        public DataInitializerPriority Priority()
        {
            return DataInitializerPriority.Medium;
        }

        public void Initialize(DbContext ezDbContext)
        {
            var backgroundTask = new BackgroundTask
            {
                Name = GetName(),
                Description = GetName(),
                Status = BackgroundTaskEnums.TaskStatus.Active,
                ScheduleType = BackgroundTaskEnums.ScheduleType.Interval,
                Interval = 60
            };
            ezDbContext.Set<BackgroundTask>().AddIfNotExist(t => t.Name, backgroundTask);
        }

        #endregion
    }
}
