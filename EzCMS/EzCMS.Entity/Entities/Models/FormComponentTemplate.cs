using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class FormComponentTemplate : BaseModel
    {
        [StringLength(60)]
        public string Name { get; set; }

        public string Content { get; set; }

        public virtual ICollection<FormComponent> FormComponents { get; set; }

        public virtual ICollection<FormDefaultComponent> FormDefaultComponents { get; set; }
    }
}