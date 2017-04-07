using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.IoC.Attributes;
using System;

namespace Ez.Framework.Core.BackgroundTasks.Base
{
    [Register(Lifetime.PerInstance)]
    public interface IEzBackgroundTaskService
    {
        /// <summary>
        /// Update last running time of task
        /// </summary>
        /// <param name="backgroundTaskType"></param>
        void UpdateLastRunningTimeTask(Type backgroundTaskType);

        /// <summary>
        /// Get task by type
        /// </summary>
        /// <param name="backgroundTaskType"></param>
        BackgroundTask GetByType(Type backgroundTaskType);

        /// <summary>
        /// Get task by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        BackgroundTask GetByName(string name);
    }
}
