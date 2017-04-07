using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Testimonial : BaseModel
    {
        [StringLength(512)]
        public string Author { get; set; }

        [StringLength(512)]
        public string AuthorDescription { get; set; }

        [StringLength(512)]
        public string AuthorImageUrl { get; set; }

        public string Content { get; set; }
    }
}