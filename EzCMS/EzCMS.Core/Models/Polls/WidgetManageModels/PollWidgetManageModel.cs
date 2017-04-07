using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.Polls;
using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Polls.WidgetManageModels
{
    public class PollWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public PollWidgetManageModel()
        {
        }

        public PollWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var pollService = HostContainer.GetInstance<IPollService>();
            Polls = pollService.GetPolls();
            ParseParams(parameters);
        }

        #region Private Methods

        /// <summary>
        /// Parse params
        /// </summary>
        /// <param name="parameters"></param>
        private void ParseParams(string[] parameters)
        {
            /*
              * Params:
              * * PollId
              * * Template 
            */

            //PollId
            if (parameters.Length > 1)
            {
                PollId = parameters[1].ToInt(0);
            }

            //Template 
            if (parameters.Length > 2)
            {
                Template = parameters[2];
            }
        }

        #endregion


        #endregion

        #region Public Properties

        [RequiredInteger("Widget_Poll_Field_PollId")]
        [LocalizedDisplayName("Widget_Poll_Field_PollId")]
        public int PollId { get; set; }

        public IEnumerable<SelectListItem> Polls { get; set; }

        #endregion
    }
}
