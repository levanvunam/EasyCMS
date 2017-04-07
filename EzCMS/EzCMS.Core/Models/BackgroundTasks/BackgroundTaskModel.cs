using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.Entity.Enums;
using Ez.Framework.Models;
using System;

namespace EzCMS.Core.Models.BackgroundTasks
{
    public class BackgroundTaskModel : BaseGridModel
    {
        public BackgroundTaskModel()
        {

        }

        public BackgroundTaskModel(BackgroundTask backgroundTask)
            : base(backgroundTask)
        {
            Name = backgroundTask.Name;
            Description = backgroundTask.Description;
            LastRunningTime = backgroundTask.LastRunningTime;
            ScheduleType = backgroundTask.ScheduleType;
            Interval = backgroundTask.Interval;
            StartTime = backgroundTask.StartTime;
            Status = backgroundTask.Status;
        }

        #region Public Properties

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? LastRunningTime { get; set; }

        public BackgroundTaskEnums.ScheduleType ScheduleType { get; set; }

        public int? Interval { get; set; }

        public TimeSpan? StartTime { get; set; }

        public BackgroundTaskEnums.TaskStatus Status { get; set; }

        #endregion
    }
}
