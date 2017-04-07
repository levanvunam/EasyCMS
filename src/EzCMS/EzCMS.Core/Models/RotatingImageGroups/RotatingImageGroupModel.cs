using System.ComponentModel.DataAnnotations;
using Ez.Framework.Models;

namespace EzCMS.Core.Models.RotatingImageGroups
{
    public class RotatingImageGroupModel : BaseGridModel
    {
        [Required]
        public string Name { get; set; }

        public string Settings { get; set; }
    }
}
