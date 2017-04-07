using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class NewsNewsCategory : BaseModel
    {
        public int NewsId { get; set; }

        [ForeignKey("NewsId")]
        public virtual News News { get; set; }

        public int NewsCategoryId { get; set; }

        [ForeignKey("NewsCategoryId")]
        public virtual NewsCategory NewsCategory { get; set; }
    }
}