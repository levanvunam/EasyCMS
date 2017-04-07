using System;
using Ez.Framework.Utilities.Excel.Enums;

namespace Ez.Framework.Utilities.Excel.Attributes
{
    public class ExcelExportAttribute : Attribute
    {
        #region Public Properties

        /// <summary>
        /// Ignore export
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Specify name for export property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The priority of property when exporting
        /// </summary>
        public ExportPriority Priority { get; set; }

        #endregion
    }
}