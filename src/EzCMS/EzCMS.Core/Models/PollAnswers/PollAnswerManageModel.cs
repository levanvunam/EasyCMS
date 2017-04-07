using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.Polls;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.PollAnswers
{
    public class PollAnswerManageModel
    {
        #region Constructors

        public PollAnswerManageModel()
        {
            var pollService = HostContainer.GetInstance<IPollService>();

            PollQuestions = pollService.GetPolls();
        }

        public PollAnswerManageModel(int? pollId)
            : this ()
        {
            if (pollId.HasValue)
            {
                PollId = pollId.Value;
            }
        }

        public PollAnswerManageModel(PollAnswer pollAnswers) 
            : this()
        {
            Id = pollAnswers.Id;
            AnswerText = pollAnswers.AnswerText;
            Total = pollAnswers.Total;
            PollId = pollAnswers.PollId;
            RecordOrder = pollAnswers.RecordOrder;
        }
        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("Poll_Field_AnswerText")]
        public string AnswerText { get; set; }

        [LocalizedDisplayName("Poll_Field_Total")]
        public int Total { get; set; }

        [LocalizedDisplayName("Poll_Field_PollId")]
        public int PollId { get; set; }

        [LocalizedDisplayName("Poll_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        public IEnumerable<SelectListItem> PollQuestions { get; set; }

        #endregion
    }
}
