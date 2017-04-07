using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class LocalizedResource : BaseModel
    {
        public int LanguageId { get; set; }

        [ForeignKey("LanguageId")]
        public virtual Language Language { get; set; }

        public string TextKey { get; set; }

        public string DefaultValue { get; set; }

        public string TranslatedValue { get; set; }
    }
}