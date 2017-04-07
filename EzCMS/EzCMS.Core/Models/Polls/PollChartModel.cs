using System.Collections.Generic;
using System.Linq;
using EzCMS.Core.Models.PollAnswers.Widgets;
using EzCMS.Core.Models.Polls.Widgets;

namespace EzCMS.Core.Models.Polls
{
    public class PollChartModel
    {
        #region Constructors

        public PollChartModel()
        {
            PollAnswers = new List<PollAnswerWidget>();
            TotalVotes = 0;
        }

        public PollChartModel(PollWidget poll)
        {
            Id = poll.Id;
            PollQuestion = poll.PollQuestion;
            PollAnswers = poll.PollAnswers;
            if (poll.PollAnswers != null && poll.PollAnswers.Any())
            {
                foreach (var answer in poll.PollAnswers)
                {
                    TotalVotes += answer.Total;
                }
            }
            
        }
        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string PollQuestion { get; set; }

        public int TotalVotes { get; set; }

        public List<PollAnswerWidget> PollAnswers { get; set; }
     
        #endregion
    }
}
