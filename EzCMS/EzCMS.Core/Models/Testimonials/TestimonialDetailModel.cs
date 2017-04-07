using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Testimonials
{
    public class TestimonialDetailModel
    {
        public TestimonialDetailModel()
        {
        }

        public TestimonialDetailModel(Testimonial testimonial)
            : this()
        {
            Id = testimonial.Id;

            Testimonial = new TestimonialManageModel(testimonial);

            Created = testimonial.Created;
            CreatedBy = testimonial.CreatedBy;
            LastUpdate = testimonial.LastUpdate;
            LastUpdateBy = testimonial.LastUpdateBy;

        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("Testimonial_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("Testimonial_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("Testimonial_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("Testimonial_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public TestimonialManageModel Testimonial { get; set; }

        #endregion
    }
}
