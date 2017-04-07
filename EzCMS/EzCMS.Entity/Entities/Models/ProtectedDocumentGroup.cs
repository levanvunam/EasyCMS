using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class ProtectedDocumentGroup : BaseModel
    {
        public int ProtectedDocumentId { get; set; }

        [ForeignKey("ProtectedDocumentId")]
        public virtual ProtectedDocument ProtectedDocument { get; set; }

        public int UserGroupId { get; set; }

        [ForeignKey("UserGroupId")]
        public virtual UserGroup UserGroup { get; set; }
    }
}