using Ez.Framework.Core.BackgroundTasks.Base;
using System;
using System.Web.Http;

namespace EzCMS.Web.Areas.BackgroundTasks.Controllers
{
    public class BackgroundEntryPointController : ApiController
    {
        private readonly IBackgroundTaskManager _backgroundTaskManager;

        public BackgroundEntryPointController(IBackgroundTaskManager backgroundTaskManager)
        {
            _backgroundTaskManager = backgroundTaskManager;
        }

        public void ExcuteTask(string taskKey)
        {
            var task = Guid.Parse(taskKey);
            _backgroundTaskManager.ExecuteTask(task, null);
        }
    }
}