using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class FormDefaultComponent : BaseModel
    {
        [StringLength(60)]
        public string Name { get; set; }

        public int FormComponentTemplateId { get; set; }

        [ForeignKey("FormComponentTemplateId")]
        public virtual FormComponentTemplate FormComponentTemplate { get; set; }

        public virtual ICollection<FormDefaultComponentField> FormDefaultComponentFields { get; set; }
    }
}