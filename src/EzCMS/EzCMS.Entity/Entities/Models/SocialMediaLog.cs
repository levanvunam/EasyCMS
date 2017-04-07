using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class SocialMediaLog : BaseModel
    {
        public int SocialMediaId { get; set; }

        public int SocialMediaTokenId { get; set; }

        [ForeignKey("SocialMediaTokenId")]
        public virtual SocialMediaToken SocialMediaToken { get; set; }

        public int PageId { get; set; }

        [ForeignKey("PageId")]
        public virtual Page Page { get; set; }

        public string PostedContent { get; set; }

        public string PostedResponse { get; set; }
    }
}