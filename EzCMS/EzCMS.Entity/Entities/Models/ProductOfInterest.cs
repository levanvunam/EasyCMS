using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class ProductOfInterest : BaseModel
    {
        [StringLength(200)]
        public string Name { get; set; }

        public int? TargetCount { get; set; }
    }
}