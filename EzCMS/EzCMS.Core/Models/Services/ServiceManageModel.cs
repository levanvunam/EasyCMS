using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.Services;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Core.Models.Services
{
    public class ServiceManageModel : IValidatableObject
    {
        #region Constructors

        public ServiceManageModel()
        {
        }

        public ServiceManageModel(Service service)
            : this()
        {
            Id = service.Id;
            Name = service.Name;
            ImageUrl = service.ImageUrl;
            Description = service.Description;
            Content = service.Content;
            Status = service.Status;
        }

        #endregion

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("Service_Field_ImageUrl")]
        public string ImageUrl { get; set; }

        [Required]
        [LocalizedDisplayName("Service_Field_Name")]
        public string Name { get; set; }

        [Required]
        [LocalizedDisplayName("Service_Field_Description")]
        public string Description { get; set; }

        [Required]
        [LocalizedDisplayName("Service_Field_Content")]
        public string Content { get; set; }

        [Required]
        [LocalizedDisplayName("Service_Field_Status")]
        public ServiceEnums.ServiceStatus Status { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var serviceService = HostContainer.GetInstance<IServiceService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (serviceService.IsNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("Service_Message_ExistingName"), new[] { "Name" });
            }
        }
        #endregion
    }
}
