using System.Collections.Generic;
using EzCMS.Core.Models.PageTemplates.Logs;

namespace EzCMS.Core.Models.PageTemplates
{
    public class PageTemplateLogListingModel
    {
        #region Constructors
        public PageTemplateLogListingModel()
        {

        }
        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public int Total { get; set; }

        public bool LoadComplete { get; set; }

        public List<PageTemplateLogsModel> Logs { get; set; }

        #endregion
    }
}
