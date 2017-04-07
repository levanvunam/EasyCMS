using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Models.PollAnswers.Widgets;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EzCMS.Core.Models.Polls.Widgets
{
    public class PollWidget
    {
        #region Constructors

        public PollWidget()
        {
            PollAnswers = new List<PollAnswerWidget>();
            TotalVotes = 0;
        }

        public PollWidget(Poll poll)
            : this()
        {
            Id = poll.Id;
            PollQuestion = poll.PollQuestion;
            PollSummary = poll.PollSummary;
            IsMultiple = poll.IsMultiple;
            ThankyouText = poll.ThankyouText;
            var pollAnswers = poll.PollAnswers.OrderBy(p => p.RecordOrder).ToList();
            foreach (var pollAnswer in pollAnswers)
            {
                TotalVotes += pollAnswer.Total;
                PollAnswers.Add(new PollAnswerWidget(pollAnswer));
            }
            var cookieName = String.Format(EzCMSContants.VotedUserCookie, poll.Id);
            var pollCookie = HttpContext.Current.Request.Cookies[cookieName];
            IsVotedPoll = pollCookie != null;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string PollQuestion { get; set; }

        [UsingRaw]
        public string PollSummary { get; set; }

        public bool IsMultiple { get; set; }

        [UsingRaw]
        public string ThankyouText { get; set; }

        public int TotalVotes { get; set; }

        public List<PollAnswerWidget> PollAnswers { get; set; }

        public bool IsVotedPoll { get; set; }

        #endregion
    }
}
