using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class LinkTrackerClick : UserAgentModel
    {
        public int LinkTrackerId { get; set; }

        [ForeignKey("LinkTrackerId")]
        public virtual LinkTracker LinkTracker { get; set; }
    }
}