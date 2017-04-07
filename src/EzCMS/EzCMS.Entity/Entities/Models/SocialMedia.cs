using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    [Table("SocialMedia")]
    public class SocialMedia : BaseModel
    {
        [StringLength(255)]
        public string Name { get; set; }

        public int MaxCharacter { get; set; }

        public virtual ICollection<SocialMediaToken> SocialMediaTokens { get; set; }
    }
}