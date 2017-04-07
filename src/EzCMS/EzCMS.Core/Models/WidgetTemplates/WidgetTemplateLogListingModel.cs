using EzCMS.Core.Models.WidgetTemplates.Logs;
using System.Collections.Generic;

namespace EzCMS.Core.Models.WidgetTemplates
{
    public class WidgetTemplateLogListingModel
    {
        #region Constructors
        public WidgetTemplateLogListingModel()
        {
            Logs = new List<WidgetTemplateLogsModel>();
        }
        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public int Total { get; set; }

        public bool LoadComplete { get; set; }

        public List<WidgetTemplateLogsModel> Logs { get; set; }

        #endregion
    }
}
