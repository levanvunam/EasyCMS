using Ez.Framework.Core.BackgroundTasks.Base;
using System.Data.Entity;
using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.Entity.Enums;
using Ez.Framework.Core.Entity.Extensions;
using Ez.Framework.Core.Entity.Intialize;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Entity.Entities;

namespace EzCMS.Core.Core.BackgroundTasks.SlideInHelps
{
    public class SlideInHelpUpdatingTaskSetup : IBackgroundTaskSetup
    {
        // Background task name
        public string GetName()
        {
            return EzCMSContants.SlideInHelpUpdatingTaskName;
        }

        /// <summary>
        /// Excute function
        /// </summary>
        /// <returns></returns>
        public IBackgroundTask GetJob()
        {
            return new SlideInHelpUpdatingTask();
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
                    ScheduleType = BackgroundTaskEnums.ScheduleType.Interval,
                    Interval = 60 * 60 //1 hour
                };

                dbContext.BackgroundTasks.AddIfNotExist(t => t.Name, backgroundTask);
            }
        }

        #endregion
    }
}