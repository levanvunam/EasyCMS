using System.ComponentModel.DataAnnotations;
using Ez.Framework.Models;

namespace EzCMS.Core.Models.Scripts
{
    public class ScriptModel : BaseGridModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string DataType { get; set; }
    }
}
