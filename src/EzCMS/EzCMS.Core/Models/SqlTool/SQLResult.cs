using System;
using System.Collections.Generic;
using System.Text;

namespace EzCMS.Core.Models.SQLTool
{
    /// <summary>
    /// Represent result of executing a user SQL Request
    /// </summary>
    public class SQLResult : SQLRequest
    {
        /// <summary>
        /// Connection string to current DB that the query has been executed against.
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Total time to execute query and parse result.
        /// </summary>
        public long ProcessTime { get; set; }
        /// <summary>
        /// The parsed return data.
        /// </summary>
        public List<DataResult> ReturnData { get; set; }
        /// <summary>
        /// Exception during executing query if any
        /// </summary>
        public Exception Error { get; set; }
        /// <summary>
        /// The number of records has been affected by UPDATE / INSERT commands in the query
        /// </summary>
        public int RecordsAffected { get; set; }
        /// <summary>
        /// Recent requests
        /// </summary>
        public IEnumerable<SqlCommandHistoryModel> Histories { get; set; }
        /// <summary>
        /// List of tables in current DB
        /// </summary>
        public IEnumerable<string> Tables { get; set; }
        public override string ToString()
        {
            var doc = new StringBuilder();
            if (Error != null)
            {
                doc.AppendLine("Error: " + Error.Message);
            }
            foreach (DataResult data in ReturnData)
            {
                bool first = true;
                foreach (string col in data.ColumnNames)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        doc.Append('\t');
                    }
                    doc.Append(col);
                }
                doc.AppendLine();
                foreach (List<object> row in data.Data)
                {
                    first = true;
                    foreach (object item in row)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            doc.Append('\t');
                        }
                        if (item != null)
                        {
                            doc.Append(item.ToString().Replace('\t', ' ').Replace((char)10, ' ').Replace('\n', ' '));
                        }
                    }
                    doc.AppendLine();
                }
                doc.AppendLine("------------------------------------------------");
            }
            return doc.ToString();
        }
    }
}