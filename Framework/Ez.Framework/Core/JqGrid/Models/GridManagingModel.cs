using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Utilities;

namespace Ez.Framework.Core.JqGrid.Models
{
    public class GridManagingModel
    {
        public string Oper { get; set; }

        public GridOperation Operation
        {
            get
            {
                return Oper.ToEnum<GridOperation>();
            }
        }
    }
}
