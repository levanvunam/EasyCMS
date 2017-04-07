using System;
using System.Collections.Generic;
using EzCMS.Core.Models.Users;

namespace EzCMS.Core.Models.Pages.Logs
{
    public class PageLogsModel
    {
        public PageLogsModel()
        {
            Logs = new List<PageLogItem>();
        }

        #region Public Properties

        public List<PageLogItem> Logs { get; set; }

        public string SessionId { get; set; }

        public SimpleUserModel Creator { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int Total { get; set; }

        #endregion
    }
}
