using System.ComponentModel.DataAnnotations;
using Ez.Framework.Models;

namespace EzCMS.Core.Models.Testimonials
{
    public class TestimonialModel : BaseGridModel
    {
        [Required]
        public string Author { get; set; }

        [Required]
        public string Content { get; set; }

        public string AuthorDescription { get; set; }

        public string AuthorImageUrl { get; set; }
    }
}
