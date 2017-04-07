using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.RotatingImageGroups.WidgetManageModels;
using EzCMS.Core.Models.RotatingImageGroups.Widgets;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.RotatingImageGroups;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.RotatingImages
{
    public class RotatingImageResolver : Widget
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
                Name = "Rotating Images",
                Widget = "RotatingImages",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Get rotating images of group and render using template",
                DefaultTemplate = "Default.RotatingImages",
                Type = typeof(RotatingImagesWidget),
                ManageType = typeof(RotatingImagesWidgetManageModel),
                IsFavourite = true
            };
        }

        #endregion

        #region Private Properties
        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly IRotatingImageGroupService _rotatingImageGroupService;
        private readonly IEzCMSLocalizedResourceService _localizedResourceService;

        #endregion

        #region Constructors

        public RotatingImageResolver()
        {
            _rotatingImageGroupService = HostContainer.GetInstance<IRotatingImageGroupService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            _localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();

        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "GroupId", Order = 1, Description = "The group id of rotating images")]
        public int GroupId { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 2, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Group Id
             * * Template 
             */

            //Count
            if (parameters.Length > 1)
            {
                GroupId = parameters[1].ToInt(0);
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
                GetSetup().Widget, GroupId
            };

            if (!string.IsNullOrEmpty(Template))
            {
                parameters.Add(Template);
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

            var model = _rotatingImageGroupService.GetRotatingImageWidget(GroupId);

            if (model == null)
            {
                return _localizedResourceService.T("Widget_RotatingImage_Message_InvalidGroupId");
            }

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
