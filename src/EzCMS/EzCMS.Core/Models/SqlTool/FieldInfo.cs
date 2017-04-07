using System;

namespace EzCMS.Core.Models.SQLTool
{
    public class FieldInfo
    {
        public string FieldName { get; set; }

        public Type FieldType { get; set; }

        public int Size { get; set; }

        public bool IsKey { get; set; }

        public bool IsAutoIncrement { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsAllowNull { get; set; }

        public int NumericPrecision { get; set; }

        public int NumericScale { get; set; }
    }
}