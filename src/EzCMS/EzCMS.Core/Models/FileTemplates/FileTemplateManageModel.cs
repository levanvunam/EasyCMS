using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.PageTemplates;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;
using EzCMS.Core.Services.FileTemplates;
using EzCMS.Core.Services.Localizes;

namespace EzCMS.Core.Models.FileTemplates
{
    public class FileTemplateManageModel : IValidatableObject
    {
        private readonly IPageTemplateService _pageTemplateService;
        public FileTemplateManageModel()
        {
            _pageTemplateService = HostContainer.GetInstance<IPageTemplateService>();

            PageTemplates = _pageTemplateService.GetPageTemplateSelectListForFileTemplate();

        }

        public FileTemplateManageModel(FileTemplate template)
            : this()
        {
            Id = template.Id;
            Name = template.Name;
            Action = template.Action;
            Controller = template.Controller;
            Area = template.Area;
            Parameters = template.Parameters;
            PageTemplateId = template.PageTemplateId;
            PageTemplates = _pageTemplateService.GetPageTemplateSelectListForFileTemplate(template.Id);
        }

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("FileTemplate_Field_Name")]
        public string Name { get; set; }

        [Required]
        [LocalizedDisplayName("FileTemplate_Field_Controller")]
        public string Controller { get; set; }

        [Required]
        [LocalizedDisplayName("FileTemplate_Field_Action")]
        public string Action { get; set; }

        [LocalizedDisplayName("FileTemplate_Field_Area")]
        public string Area { get; set; }

        [LocalizedDisplayName("FileTemplate_Field_Parameters")]
        public string Parameters { get; set; }

        [LocalizedDisplayName("FileTemplate_Field_PageTemplateId")]
        public int? PageTemplateId { get; set; }

        public IEnumerable<SelectListItem> PageTemplates { get; set; }
        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var fileTemplateService = HostContainer.GetInstance<IFileTemplateService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (fileTemplateService.IsFileTemplateNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("FileTemplate_Message_ExistingName"), new[] { "Name" });
            }
        }
    }
}
