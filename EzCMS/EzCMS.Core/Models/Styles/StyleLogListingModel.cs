using System.Collections.Generic;
using EzCMS.Core.Models.Styles.Logs;

namespace EzCMS.Core.Models.Styles
{
    public class StyleLogListingModel
    {
        #region Constructors
        public StyleLogListingModel()
        {

        }
        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public int Total { get; set; }

        public bool LoadComplete { get; set; }

        public List<StyleLogsModel> Logs { get; set; }

        #endregion
    }
}
