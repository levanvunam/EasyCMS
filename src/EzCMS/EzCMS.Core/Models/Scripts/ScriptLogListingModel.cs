using System.Collections.Generic;
using EzCMS.Core.Models.Scripts.Logs;

namespace EzCMS.Core.Models.Scripts
{
    public class ScriptLogListingModel
    {
        #region Constructors
        public ScriptLogListingModel()
        {

        }
        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public int Total { get; set; }

        public bool LoadComplete { get; set; }

        public List<ScriptLogsModel> Logs { get; set; }

        #endregion
    }
}
