using Ez.Framework.Utilities.Excel.Enums;

namespace Ez.Framework.Utilities.Excel.Models
{
    public class ExcelProperty
    {
        public string DisplayName { get; set; }

        public string Name { get; set; }

        public ExportPriority Priority { get; set; }
    }
}