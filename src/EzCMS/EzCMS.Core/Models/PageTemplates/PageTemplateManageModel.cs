using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.IoC;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Pages.Widgets;
using EzCMS.Core.Services.Widgets;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.PageTemplates;
using EzCMS.Entity.Entities.Models;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.PageTemplates
{
    public class PageTemplateManageModel : IValidatableObject
    {
        private readonly IPageTemplateService _pageTemplateService;

        public PageTemplateManageModel()
        {
            _pageTemplateService = HostContainer.GetInstance<IPageTemplateService>();
            Content = EzCMSContants.RenderBody;
            Parents = _pageTemplateService.GetPossibleParents();
        }

        public PageTemplateManageModel(PageTemplate template)
            : this()
        {

            Id = template.Id;
            Name = template.Name;
            Content = template.Content;
            IsDefaultTemplate = template.IsDefaultTemplate;
            ParentId = template.ParentId;
            Parents = _pageTemplateService.GetPossibleParents(template.Id);
        }

        public PageTemplateManageModel(PageTemplateLog log)
            : this()
        {
            Id = log.PageTemplateId;
            Name = log.Name;
            Content = log.Content;
            ParentId = log.ParentId;
            Parents = _pageTemplateService.GetPossibleParents(log.PageTemplateId);
        }

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("PageTemplate_Field_Name")]
        public string Name { get; set; }

        [Required]
        [LocalizedDisplayName("PageTemplate_Field_Content")]
        public string Content { get; set; }

        [LocalizedDisplayName("PageTemplate_Field_IsDefaultTemplate")]
        public bool IsDefaultTemplate { get; set; }

        [LocalizedDisplayName("PageTemplate_Field_ParentId")]
        public int? ParentId { get; set; }

        public IEnumerable<SelectListItem> Parents { get; set; }
        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var widgetService = HostContainer.GetInstance<IWidgetService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();

            if (_pageTemplateService.IsPageTemplateNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("PageTemplate_Message_ExistingName"), new[] { "Name" });
            }

            //Check if content is valid
            if (widgetService.IsPageTemplateValid(Content))
            {
                yield return new ValidationResult(localizedResourceService.T("PageTemplate_Message_InvalidContentFormat"), new[] { "Content" });
            }

            var razorValidMessage = string.Empty;
            try
            {
                var cacheName = Name.GetTemplateCacheName(Content);

                // Template is updated here
                EzRazorEngineHelper.TryCompileAndAddTemplate(Content, cacheName, typeof(PageRenderModel), ResolveType.Layout);
            }
            catch (TemplateParsingException exception)
            {
                razorValidMessage = exception.Message;
            }
            catch (TemplateCompilationException exception)
            {
                razorValidMessage = string.Join("\n", exception.CompilerErrors.Select(e => e.ErrorText));
            }
            catch (Exception exception)
            {
                razorValidMessage = exception.Message;
            }
            if (!string.IsNullOrEmpty(razorValidMessage))
                yield return new ValidationResult(string.Format(localizedResourceService.T("PageTemplate_Message_TemplateCompileFailure"), razorValidMessage), new[] { "Content" });
        }
    }
}
