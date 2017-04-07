using Ez.Framework.Core.Entity.Enums;
using Ez.Framework.Core.Entity.Models;
using System;

namespace Ez.Framework.Core.Entity.Entities
{
    public class BackgroundTask : BaseModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public BackgroundTaskEnums.ScheduleType ScheduleType { get; set; }

        public int? Interval { get; set; }

        public TimeSpan? StartTime { get; set; }

        public BackgroundTaskEnums.TaskStatus Status { get; set; }

        public DateTime? LastRunningTime { get; set; }
    }
}