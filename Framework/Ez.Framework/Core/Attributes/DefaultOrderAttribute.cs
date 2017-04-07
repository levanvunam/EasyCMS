using Ez.Framework.Core.JqGrid.Enums;
using System;

namespace Ez.Framework.Core.Attributes
{
    public class DefaultOrderAttribute : Attribute
    {
        public GridSort Order { get; set; }

        public OrderPriority Priority { get; set; }
    }

    public enum OrderPriority
    {
        High = 1,
        Medium = 2,
        Low = 3
    }
}
