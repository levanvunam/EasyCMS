using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ez.Framework.Core.JqGrid.Models
{
    public class ModelOptions
    {
        #region Constructors
        public ModelOptions()
        {

        }

        public ModelOptions(GridColumnType columnType, string fieldName, IEnumerable<SelectListItem> data)
        {
            switch (columnType)
            {
                case GridColumnType.Select:
                    FieldName = fieldName;
                    stype = GridColumnType.Select.GetEnumName();
                    searchoptions = new SearchOptions
                    {
                        sopt = new[] { "eq" },
                        value = GenerateSelectList(data)
                    };
                    break;
            }
        }

        private string GenerateSelectList(IEnumerable<SelectListItem> data)
        {
            var dataList = data.ToList().Select(s => string.Format("{0}:{1}", s.Value, s.Text)).ToList();
            return string.Join(";", dataList);
        }

        #endregion

        public string FieldName { get; set; }

        public string stype { get; set; }

        public SearchOptions searchoptions { get; set; }

        public class SearchOptions
        {
            public string[] sopt { get; set; }

            public string value { get; set; }
        }
    }
}