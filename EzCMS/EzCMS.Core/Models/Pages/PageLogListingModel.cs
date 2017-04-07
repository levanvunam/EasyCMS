using System.Collections.Generic;
using EzCMS.Core.Models.Pages.Logs;

namespace EzCMS.Core.Models.Pages
{
    public class PageLogListingModel
    {
        #region Constructors

        public PageLogListingModel()
        {
            Logs = new List<PageLogsModel>();
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public int Total { get; set; }

        public bool LoadComplete { get; set; }

        public List<PageLogsModel> Logs { get; set; }

        #endregion
    }
}
