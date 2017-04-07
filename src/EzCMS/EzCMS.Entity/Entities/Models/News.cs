using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Entity.Entities.Models
{
    public class News : BaseModel
    {
        public string Title { get; set; }

        public string Abstract { get; set; }

        public string Content { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        [StringLength(512)]
        public string ImageUrl { get; set; }

        public NewsEnums.NewsStatus Status { get; set; }

        public bool IsHotNews { get; set; }

        public virtual ICollection<NewsNewsCategory> NewsNewsCategories { get; set; }

        public virtual ICollection<NewsRead> NewsReads { get; set; }
    }
}