using System.ComponentModel.DataAnnotations.Schema;
using Ez.Framework.Core.Entity.Models;

namespace EzCMS.Entity.Entities.Models
{
    public class FormDefaultComponentField : FormComponentFieldBase
    {
        public int FormDefaultComponentId { get; set; }

        [ForeignKey("FormDefaultComponentId")]
        public virtual FormDefaultComponent FormDefaultComponent { get; set; }
    }
}