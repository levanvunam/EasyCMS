using Ez.Framework.Core.Entity.Intialize;
using Ez.Framework.Core.IoC.Attributes;

namespace Ez.Framework.Core.BackgroundTasks.Base
{
    [Register(Lifetime.PerInstance)]
    public interface IBackgroundTaskSetup : IDataInitializer
    {
        /// <summary>
        /// The name of task
        /// </summary>
        /// <returns></returns>
        string GetName();

        /// <summary>
        /// Get the job to run
        /// </summary>
        /// <returns></returns>
        IBackgroundTask GetJob();
    }
}
