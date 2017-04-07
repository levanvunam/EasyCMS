using System.Collections.Generic;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class Poll : BaseModel
    {
        public string PollQuestion { get; set; }

        public string PollSummary { get; set; }

        public bool IsMultiple { get; set; }

        public string ThankyouText { get; set; }

        public virtual ICollection<PollAnswer> PollAnswers { get; set; }
    }
}