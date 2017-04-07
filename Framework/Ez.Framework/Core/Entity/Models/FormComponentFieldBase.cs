using System.ComponentModel.DataAnnotations;

namespace Ez.Framework.Core.Entity.Models
{
    public class FormComponentFieldBase : BaseModel
    {
        [StringLength(60)]
        public string Name { get; set; }

        public string Attributes { get; set; }
    }
}
