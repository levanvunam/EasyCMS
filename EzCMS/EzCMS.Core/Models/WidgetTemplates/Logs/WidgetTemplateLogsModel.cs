using EzCMS.Core.Models.Users;
using System;
using System.Collections.Generic;

namespace EzCMS.Core.Models.WidgetTemplates.Logs
{
    public class WidgetTemplateLogsModel
    {
        public List<WidgetTemplateLogItem> Logs { get; set; }

        public string SessionId { get; set; }

        public SimpleUserModel Creator { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int Total { get; set; }
    }
}
