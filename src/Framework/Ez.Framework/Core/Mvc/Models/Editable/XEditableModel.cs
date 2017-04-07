using System.ComponentModel.DataAnnotations;

namespace Ez.Framework.Core.Mvc.Models.Editable
{
    public class XEditableModel
    {
        [Required]
        public int Pk { get; set; }

        [Required]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}