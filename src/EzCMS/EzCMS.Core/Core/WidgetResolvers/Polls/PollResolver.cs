using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Polls.WidgetManageModels;
using EzCMS.Core.Models.Polls.Widgets;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.Polls;
using System.Collections.Generic;
using System.Web;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.Polls
{
    public class PollResolver : Widget
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
                Name = "Poll",
                Widget = "Poll",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Get poll with poll Id and render with template",
                DefaultTemplate = "Default.Poll",
                Type = typeof(PollWidget),
                ManageType = typeof(PollWidgetManageModel),
                IsFavourite = true
            };
        }

        #endregion

        #region Private Properties

        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly IPollService _pollService;
        private readonly IEzCMSLocalizedResourceService _localizedResourceService;

        #endregion

        #region Constructors

        public PollResolver()
        {
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            _pollService = HostContainer.GetInstance<IPollService>();
            _localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "PollId", Order = 1, Description = "The Id of poll. This field can be empty, if it's empty or equal 0, system will check the url of page for PollID parameter ")]
        public int PollId { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 2, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

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

            if (PollId == 0)
            {
                PollId = HttpContext.Current.Request.QueryString["PollId"].ToInt(0);
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
                parameters.AddRange(new List<object> { PollId, Template });
            }
            else if (PollId > 0)
            {
                parameters.AddRange(new List<object> { PollId });
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

            var model = _pollService.GetPollsWidget(PollId);

            if (model == null)
            {
                return _localizedResourceService.T("Widget_Poll_Message_InvalidPollId");
            }

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                             _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
