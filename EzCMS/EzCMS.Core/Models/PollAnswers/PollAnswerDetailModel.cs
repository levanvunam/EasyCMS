using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.PollAnswers
{
    public class PollAnswerDetailModel
    {
        #region Constructors

        public PollAnswerDetailModel()
        {
            PollAnswer = new PollAnswerManageModel();
        }

        public PollAnswerDetailModel(PollAnswer pollAnswer)
            : this()
        {
            Id = pollAnswer.Id;
            PollAnswer = new PollAnswerManageModel(pollAnswer);
            PollQuestion = pollAnswer.Poll.PollQuestion;
            PollSummary = pollAnswer.Poll.PollSummary;
            IsMultiple = pollAnswer.Poll.IsMultiple;
            ThankyouText = pollAnswer.Poll.ThankyouText;
            Created = pollAnswer.Created;
            CreatedBy = pollAnswer.CreatedBy;
            LastUpdate = pollAnswer.LastUpdate;
            LastUpdateBy = pollAnswer.LastUpdateBy;
            PollId = pollAnswer.PollId;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public PollAnswerManageModel PollAnswer { get; set; }

        [LocalizedDisplayName("PollAnswer_Field_PollQuestion")]
        public string PollQuestion { get; set; }

        [LocalizedDisplayName("PollAnswer_Field_PollSummary")]
        public string PollSummary { get; set; }

        [LocalizedDisplayName("PollAnswer_Field_IsMultiple")]
        public bool IsMultiple { get; set; }

        [LocalizedDisplayName("PollAnswer_Field_ThankyouText")]
        public string ThankyouText { get; set; }

        [LocalizedDisplayName("PollAnswer_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("PollAnswer_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("PollAnswer_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("PollAnswer_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public int PollId { get; set; }

        #endregion
    }
}
