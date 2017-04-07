using Ez.Framework.Core.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Models.EventSchedules.WidgetManageModels;
using EzCMS.Core.Models.EventSchedules.Widgets;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Services.EventSchedules;
using EzCMS.Core.Services.WidgetTemplates;
using System.Collections.Generic;

namespace EzCMS.Core.Core.WidgetResolvers.Events
{
    public class EventSchedulesResolver : Widget
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
                Name = "Event Schedules",
                Widget = "EventSchedules",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Get event schedules and render using template",
                DefaultTemplate = "Default.EventSchedules",
                Type = typeof(EventSchedulesWidget),
                ManageType = typeof(EventSchedulesWidgetManageModel)
            };
        }

        #endregion

        #region Private Properties

        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly IEventScheduleService _eventScheduleService;

        #endregion

        #region Constructors

        public EventSchedulesResolver()
        {
            _eventScheduleService = HostContainer.GetInstance<IEventScheduleService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "CategoryId", Order = 1, Description = "The category id of schedules to display, this is optional parameter, if not defined then get all kinds of category")]
        public int? CategoryId { get; set; }

        [WidgetParam(Name = "Total", Order = 2, Description = "The total number of schedules to display, this is optional parameter, if not defined then get all schedules")]
        public int Total { get; set; }

        [WidgetParam(Name = "DateFormat", Order = 3, Description = "The format that used to render the date, this is optional parameter, if not defined then use 'd MMMM'")]
        public string DateFormat { get; set; }

        [WidgetParam(Name = "TimeFormat", Order = 4, Description = "The format that used to render the time, this is optional parameter, if not defined then use 'htt'")]
        public string TimeFormat { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 5, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * CategoryId
             * * Total
             * * DateFormat
             * * TimeFormat
             * * Template 
             */

            //CategoryId
            if (parameters.Length > 1)
            {
                CategoryId = parameters[1].ToInt(0);
            }

            //Total
            if (parameters.Length > 2)
            {
                Total = parameters[2].ToInt(0);
            }

            //DateFormat
            if (parameters.Length > 3)
            {
                DateFormat = parameters[3];
            }

            //TimeFormat
            if (parameters.Length > 4)
            {
                TimeFormat = parameters[4];
            }

            //Template 
            if (parameters.Length > 5)
            {
                Template = parameters[5];
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
                parameters.AddRange(new List<object> { CategoryId, Total, DateFormat, TimeFormat, Template });
            }
            else if (!string.IsNullOrEmpty(TimeFormat))
            {
                parameters.AddRange(new List<object> { CategoryId, Total, DateFormat, TimeFormat });
            }
            else if (!string.IsNullOrEmpty(DateFormat))
            {
                parameters.AddRange(new List<object> { CategoryId, Total, DateFormat });
            }
            else if (Total > 0)
            {
                parameters.AddRange(new List<object> { CategoryId, Total });
            }
            else if (CategoryId > 0)
            {
                parameters.AddRange(new List<object> { CategoryId });
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

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            var model = _eventScheduleService.GetEventSchedulesWidget(CategoryId, Total, DateFormat, TimeFormat);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
