using System;
using System.Collections.Generic;
using EzCMS.Core.Models.Users;

namespace EzCMS.Core.Models.PageTemplates.Logs
{
    public class PageTemplateLogsModel
    {
        public List<PageTemplateLogItem> Logs { get; set; }

        public string SessionId { get; set; }

        public SimpleUserModel Creator { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int Total { get; set; }
    }
}
