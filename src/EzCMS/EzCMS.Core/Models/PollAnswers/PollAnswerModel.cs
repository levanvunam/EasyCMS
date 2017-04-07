using Ez.Framework.Models;

namespace EzCMS.Core.Models.PollAnswers
{
    public class PollAnswerModel : BaseGridModel
    {
        #region Public Properties

        public string AnswerText { get; set; }

        public int Total { get; set; }

        public int PollId { get; set; }

        public string PollQuestion { get; set; }
     
        #endregion
    }
}
