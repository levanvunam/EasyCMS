using System.ComponentModel.DataAnnotations;

namespace EzCMS.Core.Models.SQLTool
{
    /// <summary>
    /// Represent a user request to executing SQL statement against current DB.
    /// </summary>
    public class SQLRequest
    {
        /// <summary>
        /// SQL Statement to be executed
        /// </summary>
        [DataType(DataType.MultilineText)]
        public string Query { get; set; }
        /// <summary>
        /// Should the query be executed in read only mode, mean executing this query does not make any change DB
        /// </summary>
        [Display(Name = "Read Only")]
        public bool ReadOnly { get; set; }
        /// <summary>
        /// Should output be server-encoded.
        /// </summary>
        [Display(Name = "Html Encode")]
        public bool HtmlEncode { get; set; }
        /// <summary>
        /// Should query will be save into History
        /// </summary>
        [Display(Name = "Save To History")]
        public bool SaveToHistory { get; set; }
    }
}