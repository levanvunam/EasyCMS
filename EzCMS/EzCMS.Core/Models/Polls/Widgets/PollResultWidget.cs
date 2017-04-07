using System.Collections.Generic;
using EzCMS.Core.Models.PollAnswers.Widgets;

namespace EzCMS.Core.Models.Polls.Widgets
{
    public class PollResultWidget
    {
        #region Constructors

        public PollResultWidget()
        {
            PollAnswers = new List<PollAnswerWidget>();
            TotalVotes = 0;
        }

        #endregion

        #region Public Properties

        public string PollQuestion { get; set; }

        public int TotalVotes { get; set; }

        public string PollResultId { get; set; }

        public List<PollAnswerWidget> PollAnswers { get; set; }

        #endregion
    }
}
