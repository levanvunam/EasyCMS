using Ez.Framework.IoC;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Polls.WidgetManageModels;
using EzCMS.Core.Models.Polls.Widgets;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.PollAnswers;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.Polls
{
    public class PollResultResolver : Widget
    {
        #region Setup

        /// <summary>
        /// Get widget information
        /// </summary>
        /// <returns></returns>
        public override WidgetSetupModel GetSetup()
        {
            return new WidgetSetupModel
            {
                Name = "Poll Result",
                Widget = "PollResult",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Get result of poll answers and render with template",
                DefaultTemplate = "Default.PollResult",
                Type = typeof(PollResultWidget),
                ManageType = typeof(PollResultWidgetManageModel),
                IsFavourite = true
            };
        }

        #endregion

        #region Private Properties

        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly IPollAnswerService _pollAnswerService;
        private readonly IEzCMSLocalizedResourceService _localizedResourceService;

        #endregion

        #region Constructors

        public PollResultResolver()
        {
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            _pollAnswerService = HostContainer.GetInstance<IPollAnswerService>();
            _localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "PollAnswerIdsString", Order = 1, Description = "The Ids of poll answers to display result, Ids separated by commas")]
        public string PollAnswerIdsString { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 2, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

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
                PollAnswerIdsString = parameters[1];
            }

            //Template 
            if (parameters.Length > 2)
            {
                Template = parameters[2];
            }
        }

        #endregion

        #region Generate Widget

        /// <summary>
        /// Generate full widget from params
        /// </summary>
        /// <returns></returns>
        public override string GenerateFullWidget()
        {
            var parameters = new List<object>
            {
                GetSetup().Widget
            };

            if (!string.IsNullOrEmpty(Template))
            {
                parameters.AddRange(new List<object> { PollAnswerIdsString, Template });
            }
            else if (!string.IsNullOrEmpty(PollAnswerIdsString))
            {
                parameters.AddRange(new List<object> { PollAnswerIdsString });
            }

            return string.Join(EzCMSContants.WidgetSeparator, parameters).ToWidgetFormat();
        }

        #endregion

        /// <summary>
        /// Render widget
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override string Render(string[] parameters)
        {
            ParseParams(parameters);

            var model = _pollAnswerService.GetPollResultsWidget(PollAnswerIdsString);

            if (model == null)
            {
                return _localizedResourceService.T("Widget_PollResult_Message_InvalidPollAnswerIds");
            }

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                            _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
