using System.ComponentModel.DataAnnotations;
using Ez.Framework.Models;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Core.Models.Services
{
    public class ServiceModel : BaseGridModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string Content { get; set; }

        public string ImageUrl { get; set; }

        public ServiceEnums.ServiceStatus Status { get; set; }
    }
}
