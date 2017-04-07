using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Link : BaseModel
    {
        [StringLength(512)]
        public string Name { get; set; }

        public string Url { get; set; }

        public string UrlTarget { get; set; }

        public string Description { get; set; }

        public int LinkTypeId { get; set; }

        [ForeignKey("LinkTypeId")]
        public LinkType LinkType { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }
    }
}