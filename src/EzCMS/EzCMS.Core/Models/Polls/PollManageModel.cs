using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Polls
{
    public class PollManageModel
    {
        #region Constructors

        public PollManageModel()
        {
            
        }

        public PollManageModel(Poll poll) : this()
        {
            Id = poll.Id;
            PollQuestion = poll.PollQuestion;
            PollSummary = poll.PollSummary;
            IsMultiple = poll.IsMultiple;
            ThankyouText = poll.ThankyouText;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("Poll_Field_PollQuestion")]
        public string PollQuestion { get; set; }

        [LocalizedDisplayName("Poll_Field_PollSummary")]
        public string PollSummary { get; set; }

        [LocalizedDisplayName("Poll_Field_IsMultiple")]
        public bool IsMultiple { get; set; }

        [LocalizedDisplayName("Poll_Field_ThankyouText")]
        public string ThankyouText { get; set; }

        #endregion
    }
}
