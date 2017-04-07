using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.ProtectedDocuments.WidgetManageModels;
using EzCMS.Core.Models.ProtectedDocuments.Widgets;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.ProtectedDocuments;
using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.WidgetTemplates;

namespace EzCMS.Core.Core.WidgetResolvers.ProtectedDocuments
{
    public class ProtectedDocumentFilesResolver : Widget
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
                Name = "Protected Document Files",
                Widget = "ProtectedDocumentFiles",
                Description = "Render list of files inside a protected document folder using template",
                DefaultTemplate = "Default.ProtectedDocumentFiles",
                Type = typeof(ProtectedDocumentFilesWidget),
                ManageType = typeof(ProtectedDocumentFilesWidgetManageModel)
            };
        }

        #endregion

        #region Private Properties
        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly IDocumentService _documentService;
        private readonly IEzCMSLocalizedResourceService _localizedResourceService;

        #endregion

        #region Constructors
        public ProtectedDocumentFilesResolver()
        {
            _documentService = HostContainer.GetInstance<IDocumentService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
            _localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "RelativePath", Order = 1, Description = "The relative path of protected documents want to show")]
        public string Path { get; set; }

        [WidgetParam(Name = "Total", Order = 2, Description = "Get total files in folder")]
        public int Total { get; set; }

        [WidgetParam(Name = "TemplateName", Order = 3, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Relative Path
             * * Total
             * * Template 
             */

            //RelativePath
            if (parameters.Length > 1)
            {
                Path = parameters[1];
            }

            //Total
            if (parameters.Length > 2)
            {
                Total = parameters[2].ToInt(0);
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
                parameters.AddRange(new List<object> { Path, Total, Template });
            }
            else if (Total > 0)
            {
                parameters.AddRange(new List<object> { Path, Total });
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

            var model = _documentService.GetProtectedDocumentFilesWidget(Path, Total);

            if (model == null)
            {
                return _localizedResourceService.T("Widget_ProtectedDocumentFiles_Message_InvalidPath");
            }

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
