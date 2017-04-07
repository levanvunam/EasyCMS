using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class LinkTracker : BaseModel
    {
        public string Name { get; set; }

        public bool IsAllowMultipleClick { get; set; }

        public string RedirectUrl { get; set; }

        public int? PageId { get; set; }

        [ForeignKey("PageId")]
        public virtual Page Page { get; set; }

        public virtual ICollection<LinkTrackerClick> LinkTrackerClicks { get; set; }
    }
}