using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Forms.WidgetManageModels;
using EzCMS.Core.Models.Forms.Widgets;
using EzCMS.Core.Services.Forms;
using EzCMS.Core.Services.Localizes;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.SiteSettings;

namespace EzCMS.Core.Core.WidgetResolvers.Forms
{
    public class FormResolver : Widget
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
                Name = "Form Render",
                Widget = "Form",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Get form and render",
                Type = typeof(FormWidget),
                ManageType = typeof(FormWidgetManageModel),
                IsFavourite = true
            };
        }

        #endregion

        #region Private Properties
        private readonly ISiteSettingService _siteSettingService;
        private readonly IFormService _formService;
        private readonly IEzCMSLocalizedResourceService _localizedResourceService;

        #endregion

        #region Constructors

        public FormResolver()
        {
            _formService = HostContainer.GetInstance<IFormService>();
            _siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
            _localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "FormId", Order = 1, Description = "The id of Form that need to to get")]
        public int FormId { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Form Id
             */

            //Count
            if (parameters.Length > 1)
            {
                FormId = parameters[1].ToInt();
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
                GetSetup().Widget, FormId
            };

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

            var form = _formService.GetById(FormId);

            if (form == null)
            {
                return _localizedResourceService.T("Widget_Form_Message_InvalidFormId");
            }

            var formRenderModel = new FormWidget(form);

            var formBuilderSetting = _siteSettingService.LoadSetting<FormBuilderSetting>();
            var cacheName = SettingNames.FormBuilderSetting.GetTemplateCacheName(formBuilderSetting.EmbeddedTemplate);

            return RazorEngineHelper.CompileAndRun(formBuilderSetting.EmbeddedTemplate, formRenderModel, null, cacheName);
        }
    }
}
