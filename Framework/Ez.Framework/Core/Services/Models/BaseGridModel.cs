using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Entity.Models;
using Ez.Framework.Utilities.Excel.Attributes;
using Ez.Framework.Utilities.Excel.Enums;
using System;

namespace Ez.Framework.Models
{
    public class BaseGridModel
    {
        public BaseGridModel()
        {

        }

        public BaseGridModel(BaseModel model)
            : this()
        {
            RecordOrder = model.RecordOrder;
            Created = model.Created;
            CreatedBy = model.CreatedBy;
            LastUpdate = model.LastUpdate;
            LastUpdateBy = model.LastUpdateBy;
        }

        #region Public Properties

        [ExcelExport(Priority = ExportPriority.High)]
        public int Id { get; set; }

        [DefaultOrder(Priority = OrderPriority.Medium)]
        [ExcelExport(Name = "Record Order", Priority = ExportPriority.Low)]
        public int RecordOrder { get; set; }

        [ExcelExport(Priority = ExportPriority.Low)]
        public DateTime Created { get; set; }

        [ExcelExport(Name = "Created By", Priority = ExportPriority.Low)]
        public string CreatedBy { get; set; }

        [ExcelExport(Name = "Last Update", Priority = ExportPriority.Low)]
        public DateTime? LastUpdate { get; set; }

        [ExcelExport(Name = "Last Update By", Priority = ExportPriority.Low)]
        public string LastUpdateBy { get; set; }

        #endregion
    }
}
