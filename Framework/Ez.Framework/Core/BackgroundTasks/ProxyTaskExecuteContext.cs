using System;

namespace Ez.Framework.Core.BackgroundTasks
{
    public class ProxyTaskExecuteContext
    {
        public BackgroundTaskManager BackgroundTaskManager { get; set; }

        public Guid TaskKey { get; set; }

        public Uri Url { get; set; }
    }
}