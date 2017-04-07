using System;
using Ez.Framework.Models;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Core.Models.News
{
    public class NewsModel : BaseGridModel
    {
        public string Title { get; set; }

        public string Abstract { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        public string ImageUrl { get; set; }

        public bool IsHotNews { get; set; }

        public NewsEnums.NewsStatus Status { get; set; }

        public string Categories { get; set; }

        public int TotalReads { get; set; }
    }
}
