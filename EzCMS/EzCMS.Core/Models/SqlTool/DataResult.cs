using System.Collections.Generic;

namespace EzCMS.Core.Models.SQLTool
{
    /// <summary>
    /// Represent a table return from DB engine.
    /// </summary>
    public class DataResult
    {
        /// <summary>
        /// Column names
        /// </summary>
        public List<string> ColumnNames { get; set; }
        /// <summary>
        /// List of data row.
        /// Each row contains list of data corresponding to column names
        /// </summary>
        public List<List<object>> Data { get; set; }
    }
}