using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class FormTab : BaseModel
    {
        [StringLength(60)]
        public string Name { get; set; }

        public virtual ICollection<FormComponent> FormComponents { get; set; }
    }
}