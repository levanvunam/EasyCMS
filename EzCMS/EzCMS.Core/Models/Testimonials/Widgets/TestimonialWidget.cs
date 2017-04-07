using Ez.Framework.Models;
using EzCMS.Core.Framework.Configuration;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web;

namespace EzCMS.Core.Models.Testimonials.Widgets
{
    public class TestimonialWidget : BaseGridModel
    {
        [Required]
        public string Author { get; set; }

        [Required]
        public string Content { get; set; }

        public string AuthorDescription { get; set; }

        public string AuthorImageUrl { get; set; }

        public string AvatarPath
        {
            get
            {
                if (string.IsNullOrEmpty(AuthorImageUrl) || !File.Exists(HttpContext.Current.Server.MapPath(AuthorImageUrl)))
                {
                    return EzCMSContants.NoAvatar;
                }
                return AuthorImageUrl;
            }
        }
    }
}
