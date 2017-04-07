using Ez.Framework.Core.BackgroundTasks.Models;

namespace Ez.Framework.Core.BackgroundTasks.Base
{
    public interface IBackgroundTask
    {
        /// <summary>
        /// Run background task
        /// </summary>
        /// <param name="context"></param>
        void Run(BackgroundTaskExecuteContext context);
    }
}
