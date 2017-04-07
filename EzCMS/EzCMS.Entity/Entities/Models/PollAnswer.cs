using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class PollAnswer : BaseModel
    {
        public string AnswerText { get; set; }

        public int Total { get; set; }

        public int PollId { get; set; }

        [ForeignKey("PollId")]
        public virtual Poll Poll { get; set; }
    }
}