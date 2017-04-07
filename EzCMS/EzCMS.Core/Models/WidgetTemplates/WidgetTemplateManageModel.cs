using Ez.Framework.Configurations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.WidgetTemplates.TemplateBuilder;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.WidgetTemplates;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.WidgetTemplates
{
    public class WidgetTemplateManageModel : IValidatableObject
    {
        public WidgetTemplateManageModel()
        {

        }

        public WidgetTemplateManageModel(WidgetTemplate widgetTemplate)
        {
            Id = widgetTemplate.Id;
            Name = widgetTemplate.Name;
            Content = widgetTemplate.Content;
            FullContent = widgetTemplate.FullContent;
            Shortcuts = SerializeUtilities.Deserialize<List<Shortcut>>(widgetTemplate.Widgets);
            Script = widgetTemplate.Script;
            Style = widgetTemplate.Style;
            DataType = widgetTemplate.DataType;
            IsDefaultTemplate = widgetTemplate.IsDefaultTemplate;
            Widget = widgetTemplate.Widget;
        }

        public WidgetTemplateManageModel(WidgetTemplateLog log)
            : this()
        {
            Id = log.TemplateId;
            Name = log.Name;
            Shortcuts = SerializeUtilities.Deserialize<List<Shortcut>>(log.Widgets);
            Script = log.Script;
            Style = log.Style;
            Content = log.Content;
            FullContent = log.FullContent;
            DataType = log.DataType;
            IsDefaultTemplate = log.WidgetTemplate.IsDefaultTemplate;
            Widget = log.WidgetTemplate.Widget;
        }

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("WidgetTemplate_Field_Name")]
        public string Name { get; set; }

        [Required]
        public string DataType { get; set; }

        [Required]
        [LocalizedDisplayName("WidgetTemplate_Field_Content")]
        public string Content { get; set; }

        [Required]
        [LocalizedDisplayName("WidgetTemplate_Field_FullContent")]
        public string FullContent { get; set; }

        [LocalizedDisplayName("WidgetTemplate_Field_Shortcuts")]
        public List<Shortcut> Shortcuts { get; set; }

        [LocalizedDisplayName("WidgetTemplate_Field_Script")]
        public string Script { get; set; }

        [LocalizedDisplayName("WidgetTemplate_Field_Style")]
        public string Style { get; set; }

        [Required]
        public string Widget { get; set; }

        public bool IsDefaultTemplate { get; set; }
        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var templateService = HostContainer.GetInstance<IWidgetTemplateService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (templateService.IsTemplateNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("WidgetTemplate_Message_ExistingName"), new[] { "Name" });
            }

            var type = Type.GetType(DataType);
            if (type != null)
            {
                var razorValidMessage = EzRazorEngineHelper.ValidateTemplate(Name, FullContent, type);
                if (!string.IsNullOrEmpty(razorValidMessage))
                    yield return new ValidationResult(string.Format(localizedResourceService.T("WidgetTemplate_Message_FullContentCompileFailure"), razorValidMessage), new[] { "FullContent" });

                //Generate content from widgets
                var content = WidgetHelper.GetFullTemplate(Content, string.Empty, string.Empty, DataType, Shortcuts);
                razorValidMessage = EzRazorEngineHelper.ValidateTemplate(Name, content, type);
                if (!string.IsNullOrEmpty(razorValidMessage))
                    yield return new ValidationResult(string.Format(localizedResourceService.T("WidgetTemplate_Message_ContentCompileFailure"), razorValidMessage), new[] { "Content" });

                razorValidMessage = EzRazorEngineHelper.ValidateTemplate(Name, Script, type);
                if (!string.IsNullOrEmpty(razorValidMessage))
                    yield return new ValidationResult(string.Format(localizedResourceService.T("WidgetTemplate_Message_ScriptCompileFailure"), razorValidMessage), new[] { "Script" });

                razorValidMessage = EzRazorEngineHelper.ValidateTemplate(Name, Style, type);
                if (!string.IsNullOrEmpty(razorValidMessage))
                    yield return new ValidationResult(string.Format(localizedResourceService.T("WidgetTemplate_Message_StyleCompileFailure"), razorValidMessage), new[] { "Style" });

                if (Shortcuts != null)
                {
                    var widgetValidationMessages = new Dictionary<string, string>();
                    foreach (var widget in Shortcuts)
                    {
                        razorValidMessage = EzRazorEngineHelper.ValidateTemplate(widget.Name, widget.Content, type);
                        if (!string.IsNullOrEmpty(razorValidMessage))
                            widgetValidationMessages.Add(widget.Name, razorValidMessage);
                    }

                    if (widgetValidationMessages.Any())
                    {
                        var message = string.Join(FrameworkConstants.BreakLine, widgetValidationMessages.Select(widget => localizedResourceService.TFormat("WidgetTemplate_Message_WidgetCompileFailure", widget.Key, widget.Value)));
                        yield return new ValidationResult(message, new[] { "Widgets" });
                    }
                }
            }
        }
    }
}
