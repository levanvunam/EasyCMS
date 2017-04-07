using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Testimonials
{
    public class TestimonialManageModel
    {
        #region Constructors

        public TestimonialManageModel()
        {

        }

        public TestimonialManageModel(Testimonial testimonial)
            : this()
        {
            Id = testimonial.Id;
            Author = testimonial.Author;
            Content = testimonial.Content;
            AuthorDescription = testimonial.AuthorDescription;
            AuthorImageUrl = testimonial.AuthorImageUrl;
        }

        #endregion

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [StringLength(512)]
        [LocalizedDisplayName("Testimonial_Field_Author")]
        public string Author { get; set; }

        [Required]
        [LocalizedDisplayName("Testimonial_Field_Content")]
        public string Content { get; set; }

        [StringLength(512)]
        [LocalizedDisplayName("Testimonial_Field_AuthorDescription")]
        public string AuthorDescription { get; set; }

        [StringLength(512)]
        [LocalizedDisplayName("Testimonial_Field_AuthorImageUrl")]
        public string AuthorImageUrl { get; set; }

        #endregion
    }
}
