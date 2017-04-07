using System.Collections.Generic;

namespace EzCMS.Core.Models.BackgroundTasks
{
    public class BackgroundTaskLogsModel
    {
        #region Constructors

        public BackgroundTaskLogsModel()
        {
            LogsDates = new List<BackgroundTaskLogsDateModel>();
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public List<BackgroundTaskLogsDateModel> LogsDates { get; set; }

        public string BackgroundTaskName { get; set; }

        public bool LoadComplete { get; set; }

        public int Total { get; set; }

        public string NextDateLog { get; set; }

        #endregion
    }
}
