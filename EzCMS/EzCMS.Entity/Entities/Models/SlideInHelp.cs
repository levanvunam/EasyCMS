using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class SlideInHelp : BaseModel
    {
        public int LanguageId { get; set; }

        [ForeignKey("LanguageId")]
        public virtual Language Language { get; set; }

        public string TextKey { get; set; }

        public string HelpTitle { get; set; }

        public string LocalHelpContent { get; set; }

        public string MasterHelpContent { get; set; }

        public int MasterVersion { get; set; }

        public int LocalVersion { get; set; }

        public bool Active { get; set; }
    }
}