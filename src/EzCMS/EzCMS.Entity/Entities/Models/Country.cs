using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Country : BaseModel
    {
        [StringLength(512)]
        public string Name { get; set; }
    }
}