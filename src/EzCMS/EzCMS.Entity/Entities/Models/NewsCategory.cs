using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class NewsCategory : BaseHierachyModel<NewsCategory>
    {
        [StringLength(512)]
        public string Name { get; set; }

        public string Abstract { get; set; }

        public virtual ICollection<NewsCategory> ChildCategories { get; set; }

        public virtual ICollection<NewsNewsCategory> NewsNewsCategories { get; set; }
    }
}