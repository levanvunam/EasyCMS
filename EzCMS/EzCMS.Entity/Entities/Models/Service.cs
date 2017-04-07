using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Entity.Entities.Models
{
    public class Service : BaseModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        [StringLength(512)]
        public string ImageUrl { get; set; }

        public ServiceEnums.ServiceStatus Status { get; set; }
    }
}