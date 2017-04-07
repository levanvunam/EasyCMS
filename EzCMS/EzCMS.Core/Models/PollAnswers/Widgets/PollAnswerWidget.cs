using System;
using System.Linq;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.PollAnswers.Widgets
{
    public class PollAnswerWidget
    {
        #region Constructors

        public PollAnswerWidget(PollAnswer pollAnswer)
        {
            Id = pollAnswer.Id;
            AnswerText = pollAnswer.AnswerText;
            Total = pollAnswer.Total;
            var totalVotes = pollAnswer.Poll.PollAnswers.Sum(p => p.Total);
            Percentage = totalVotes > 0 ? Math.Round((pollAnswer.Total * 100.0 / totalVotes), 1) : 0.0;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string AnswerText { get; set; }

        public int Total { get; set; }

        public double Percentage { get; set; }

        #endregion
    }
}
