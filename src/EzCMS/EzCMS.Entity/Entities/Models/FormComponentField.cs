using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class FormComponentField : FormComponentFieldBase
    {
        public int FormComponentId { get; set; }

        [ForeignKey("FormComponentId")]
        public virtual FormComponent FormComponent { get; set; }
    }
}