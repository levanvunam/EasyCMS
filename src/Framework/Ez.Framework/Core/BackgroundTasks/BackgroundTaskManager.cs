using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.BackgroundTasks.Models;
using Ez.Framework.Core.Entity.Enums;
using Ez.Framework.Core.IoC;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Web;

namespace Ez.Framework.Core.BackgroundTasks
{
    public class BackgroundTaskManager : IBackgroundTaskManager
    {
        private IScheduler _scheduler;
        private readonly Dictionary<Guid, IBackgroundTask> _taskLists = new Dictionary<Guid, IBackgroundTask>();
        public const string ProxyContextKey = "ProxyContextKey";

        public void Start(Type proxyTaskType)
        {
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            var taskSetups = HostContainer.GetInstance<IEnumerable<IBackgroundTaskSetup>>();
            var backgroundTaskService = HostContainer.GetInstance<IEzBackgroundTaskService>();

            foreach (var backgroundTaskSetup in taskSetups)
            {
                // Get task configuration
                var configuration = backgroundTaskService.GetByName(backgroundTaskSetup.GetName());

                // Only start active task
                if (configuration.Status != BackgroundTaskEnums.TaskStatus.Disabled)
                {
                    #region Get Job

                    var realTask = backgroundTaskSetup.GetJob();
                    var realTaskKey = Guid.NewGuid();
                    _taskLists.Add(realTaskKey, realTask);

                    var proxyContext = new ProxyTaskExecuteContext
                    {
                        BackgroundTaskManager = this,
                        TaskKey = realTaskKey,
                        Url = HttpContext.Current == null ? null : HttpContext.Current.Request.Url
                    };

                    IJobDetail job = JobBuilder.Create(proxyTaskType)
                        .UsingJobData(new JobDataMap
                    {
                        new KeyValuePair<string, object>(ProxyContextKey, proxyContext)
                    }).Build();

                    #endregion

                    #region Get Trigger

                    // Get the task trigger setup
                    ITrigger trigger = null;
                    switch (configuration.ScheduleType)
                    {
                        case BackgroundTaskEnums.ScheduleType.Interval:
                            if (configuration.Interval.HasValue)
                            {
                                trigger = TriggerBuilder.Create()
                                    .StartNow()
                                    .WithSimpleSchedule(x => x
                                        .WithIntervalInSeconds(configuration.Interval.Value)
                                        .RepeatForever())
                                    .Build();
                            }
                            break;

                        case BackgroundTaskEnums.ScheduleType.Daily:
                            if (configuration.StartTime.HasValue)
                            {
                                var hour = configuration.StartTime.Value.Hours;
                                var minutes = configuration.StartTime.Value.Minutes;
                                trigger = TriggerBuilder.Create()
                                    .WithDailyTimeIntervalSchedule
                                    (s =>
                                        s.WithIntervalInHours(24)
                                            .OnEveryDay()
                                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minutes))
                                            .InTimeZone(TimeZoneInfo.Utc)
                                    )
                                    .Build();
                            }
                            break;
                    }

                    // Only start active task
                    if (trigger != null)
                    {
                        _scheduler.ScheduleJob(job, trigger);
                    }

                    #endregion

                    backgroundTaskSetup.GetName();
                }
            }
            _scheduler.Start();
        }

        public void ExecuteTask(Guid taskKey, BackgroundTaskExecuteContext context)
        {
            IBackgroundTask realTask;
            if (_taskLists.TryGetValue(taskKey, out realTask))
            {
                realTask.Run(context);
            }
        }

        public void Shutdown()
        {
            if (_scheduler == null)
            {
                throw new InvalidOperationException("Cannot shutdown scheduler since it has not been started.");
            }
        }
    }
}