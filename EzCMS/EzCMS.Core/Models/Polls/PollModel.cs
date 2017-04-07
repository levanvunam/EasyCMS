using Ez.Framework.Models;

namespace EzCMS.Core.Models.Polls
{
    public class PollModel : BaseGridModel
    {
        #region Public Properties

        public string PollQuestion { get; set; }

        public string PollSummary { get; set; }

        public bool IsMultiple { get; set; }

        public string ThankyouText { get; set; }
     
        #endregion
    }
}
