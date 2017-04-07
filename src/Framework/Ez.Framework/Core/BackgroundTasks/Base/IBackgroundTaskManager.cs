using Ez.Framework.Core.BackgroundTasks.Models;
using Ez.Framework.Core.IoC.Attributes;
using System;

namespace Ez.Framework.Core.BackgroundTasks.Base
{
    [Register(Lifetime.SingleTon)]
    public interface IBackgroundTaskManager
    {
        void Start(Type proxyTaskType);

        void ExecuteTask(Guid taskKey, BackgroundTaskExecuteContext context);

        void Shutdown();
    }
}