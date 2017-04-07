using Ez.Framework.IoC;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.ProtectedDocuments.WidgetManageModels;
using EzCMS.Core.Models.ProtectedDocuments.Widgets;
using EzCMS.Core.Services.ProtectedDocuments;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.ProtectedDocuments
{
    public class ProtectedDocumentsResolver : Widget
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
                Name = "Protected Documents",
                Widget = "ProtectedDocuments",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Render protected documents with template",
                DefaultTemplate = "Default.ProtectedDocuments",
                Type = typeof(ProtectedDocumentWidget),
                ManageType = typeof(ProtectedDocumentWidgetManageModel)
            };
        }

        #endregion

        #region Private Properties
        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly IDocumentService _documentService;

        #endregion

        #region Constructors
        public ProtectedDocumentsResolver()
        {
            _documentService = HostContainer.GetInstance<IDocumentService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "RelativePath", Order = 1, Description = "The relative path of protected documents want to show")]
        public string Path { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 2, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Relative Path
             * * Template 
             */

            //Count
            if (parameters.Length > 1)
            {
                Path = parameters[1];
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
                parameters.AddRange(new List<object> { Path, Template });
            }
            else if (!string.IsNullOrEmpty(Path))
            {
                parameters.AddRange(new List<object> { Path });
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

            var model = _documentService.GetProtectedDocumentWidget(Path);

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
