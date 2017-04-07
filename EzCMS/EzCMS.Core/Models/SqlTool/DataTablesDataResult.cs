namespace EzCMS.Core.Models.SQLTool
{
    /// <summary>
    /// Represent a datatables result.
    /// </summary>
    public class DataTablesDataResult
    {
        public string sEcho { get; set; }

        public int iTotalRecords { get; set; }

        public int iTotalDisplayRecords { get; set; }

        public dynamic aaData { get; set; }
    }
}