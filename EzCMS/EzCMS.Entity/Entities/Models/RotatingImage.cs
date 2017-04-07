using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class RotatingImage : BaseModel
    {
        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(512)]
        public string ImageUrl { get; set; }

        public string Text { get; set; }

        [StringLength(512)]
        public string Url { get; set; }

        public string UrlTarget { get; set; }

        public int GroupId { get; set; }

        [ForeignKey("GroupId")]
        public virtual RotatingImageGroup RotatingImageGroup { get; set; }
    }
}