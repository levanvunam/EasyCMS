using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.Polls;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.PollAnswers
{
    public class PollAnswerSearchModel
    {
        #region Constructors

        public PollAnswerSearchModel()
        {
            var pollService = HostContainer.GetInstance<IPollService>();
            Polls = pollService.GetPolls();
        }

        #endregion

        #region Public Properties

        public string Keyword { get; set; }

        public int? PollId { get; set; }

        public IEnumerable<SelectListItem> Polls { get; set; }

        #endregion
    }
}
