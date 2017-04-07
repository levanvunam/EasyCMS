using Ez.Framework.Models;
using System.ComponentModel.DataAnnotations;

namespace EzCMS.Core.Models.WidgetTemplates
{
    public class WidgetTemplateModel : BaseGridModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string DataType { get; set; }

        public string Widget { get; set; }

        public bool IsDefaultTemplate { get; set; }
    }
}
