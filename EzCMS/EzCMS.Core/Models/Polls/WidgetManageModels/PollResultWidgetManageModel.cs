using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.PollAnswers;
using EzCMS.Core.Services.Polls;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Polls.WidgetManageModels
{
    public class PollResultWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public PollResultWidgetManageModel()
        {
        }

        public PollResultWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var pollServerice = HostContainer.GetInstance<IPollService>();
            var pollAnswerService = HostContainer.GetInstance<IPollAnswerService>();

            ParseParams(parameters);

            Polls = pollServerice.GetPolls();

            var poll = pollServerice.GetPoll(PollAnswerIdsString);
            PollId = poll != null ? poll.Id : 0;

            var pollAnswers = pollAnswerService.GetPollAnswerWidgets(PollId);
            if (pollAnswers != null)
            {
                PollAnswers = pollAnswers.Select(x => new SelectListItem
                {
                    Text = x.AnswerText,
                    Value = x.Id.ToString(),
                });
            }
        }

        #endregion

        #region Parse Params

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * PollAnswerIds
             * * Template 
             */

            //PollAnswerIds
            if (parameters.Length > 1)
            {
                //PollAnswerIdsString = parameters[1];
                PollAnswerIds = parameters[1].Split(',').Select(int.Parse).ToList();
                PollAnswerIdsString = parameters[1];
            }

            //Template 
            if (parameters.Length > 2)
            {
                Template = parameters[2];
            }

        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("Widget_PollResult_Field_PollId")]
        public int PollId { get; set; }

        [LocalizedDisplayName("Widget_PollResult_Field_PollAnswerIds")]
        public List<int> PollAnswerIds { get; set; }

        public string PollAnswerIdsString { get; set; }

        public IEnumerable<SelectListItem> Polls { get; set; }

        public IEnumerable<SelectListItem> PollAnswers { get; set; }

        #endregion
    }
}
