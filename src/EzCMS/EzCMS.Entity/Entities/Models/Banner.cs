using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Banner : BaseModel
    {
        [StringLength(1024)]
        public string ImageUrl { get; set; }

        public string Text { get; set; }

        [StringLength(512)]
        public string Url { get; set; }

        [StringLength(512)]
        public string GroupName { get; set; }
    }
}