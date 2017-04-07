using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Notices.WidgetManageModels;
using EzCMS.Core.Models.Notices.Widgets;
using EzCMS.Core.Services.Notices;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.Notices
{
    public class NoticeboardResolver : Widget
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
                Name = "Noticeboard",
                Widget = "Noticeboard",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Get noticeboard with notice type and render using template",
                DefaultTemplate = "Default.Noticeboard",
                Type = typeof(NoticeboardWidget),
                ManageType = typeof(NoticeBoardWidgetManageModel)
            };
        }

        #endregion

        #region Private Properties

        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly INoticeService _noticeService;

        #endregion

        #region Constructors

        public NoticeboardResolver()
        {
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            _noticeService = HostContainer.GetInstance<INoticeService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "NoticeTypeId", Order = 1, Description = "The Id of notice type")]
        public int NoticeTypeId { get; set; }

        [WidgetParam(Name = "Number", Order = 2, Description = "The number of notices to display")]
        public int Number { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 3, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * NoticeTypeId
             * * Number
             * * Template 
             */

            //NoticeTypeId
            if (parameters.Length > 1)
            {
                NoticeTypeId = parameters[1].ToInt(0);
            }

            //Number
            if (parameters.Length > 2)
            {
                Number = parameters[2].ToInt(0);
            }

            //Template 
            if (parameters.Length > 3)
            {
                Template = parameters[3];
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
                parameters.AddRange(new List<object> { NoticeTypeId, Number, Template });
            }
            else if (Number > 0)
            {
                parameters.AddRange(new List<object> { NoticeTypeId, Number });
            }
            else if (NoticeTypeId > 0)
            {
                parameters.AddRange(new List<object> { NoticeTypeId });
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

            var model = _noticeService.GetNoticesWidgets(NoticeTypeId, Number);

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
