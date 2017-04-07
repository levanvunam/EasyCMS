using System;
using Ez.Framework.Models;

namespace EzCMS.Core.Models.SQLTool
{
    public class SqlCommandHistoryModel : BaseGridModel
    {
        /// <summary>
        /// SQL statement that has been sent
        /// </summary>
        public string Query { get; set; }
    }
}