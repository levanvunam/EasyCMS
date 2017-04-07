using Ez.Framework.Utilities.Excel.Attributes;
using Ez.Framework.Utilities.Excel.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Ez.Framework.Core.Entity.Models
{
    /// <summary>
    /// Define base attributes each model must have
    /// </summary>
    public class BaseModel
    {
        [Key]
        [ExcelExport(Priority = ExportPriority.High)]
        public int Id { get; set; }

        [ExcelExport(Priority = ExportPriority.Low)]
        public int RecordOrder { get; set; }

        [ExcelExport(Priority = ExportPriority.Low)]
        public bool RecordDeleted { get; set; }

        [ExcelExport(Priority = ExportPriority.Low)]
        public DateTime Created { get; set; }

        [StringLength(100)]
        [ExcelExport(Priority = ExportPriority.Low)]
        public string CreatedBy { get; set; }

        [ExcelExport(Priority = ExportPriority.Low)]
        public DateTime? LastUpdate { get; set; }

        [StringLength(100)]
        [ExcelExport(Priority = ExportPriority.Low)]
        public string LastUpdateBy { get; set; }
    }
}