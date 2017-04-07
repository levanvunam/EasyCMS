using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.Entity.Enums;
using Ez.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.BackgroundTasks
{
    public class BackgroundTaskManageModel
    {
        #region Constructors

        public BackgroundTaskManageModel()
        {
            StatusList = EnumUtilities.GenerateSelectListItems<BackgroundTaskEnums.TaskStatus>();
            ScheduleTypes = EnumUtilities.GetAllItems<BackgroundTaskEnums.ScheduleType>();
        }

        public BackgroundTaskManageModel(BackgroundTask backgroundTask)
            : this()
        {
            Id = backgroundTask.Id;
            Name = backgroundTask.Name;
            Description = backgroundTask.Description;
            ScheduleType = backgroundTask.ScheduleType;
            Interval = backgroundTask.Interval;
            StartTime = backgroundTask.StartTime;
            Status = backgroundTask.Status;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("BackgroundTask_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("BackgroundTask_Field_Description")]
        public string Description { get; set; }

        [LocalizedDisplayName("BackgroundTask_Field_ScheduleType")]
        public BackgroundTaskEnums.ScheduleType ScheduleType { get; set; }

        public IEnumerable<BackgroundTaskEnums.ScheduleType> ScheduleTypes { get; set; }

        [LocalizedDisplayName("BackgroundTask_Field_Interval")]
        public int? Interval { get; set; }

        [LocalizedDisplayName("BackgroundTask_Field_StartTime")]
        public TimeSpan? StartTime { get; set; }

        [LocalizedDisplayName("BackgroundTask_Field_Status")]
        public BackgroundTaskEnums.TaskStatus Status { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }

        #endregion
    }
}
